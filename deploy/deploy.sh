#!/usr/bin/env bash
#
# 部署 nopCommerce + MySQL 8.0（首次）
#
# 用法:
#   bash deploy.sh              # 首次部署（data/ 与本脚本同级）
#   bash deploy.sh --status     # 查看状态
#   bash deploy.sh --stop       # 停止
#   bash deploy.sh --down       # 停止并删除容器（数据保留）
#   bash deploy.sh --logs       # 跟踪日志
#
# 幂等: 重复执行安全，已有数据的目录不会被覆盖。

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
COMPOSE_FILE="${SCRIPT_DIR}/docker-compose-mysql.yml"
DATA_ROOT="${SCRIPT_DIR}/data"

# 与 docker-compose-mysql.yml 中写死的值保持一致
NOP_IMAGE="nopcommerceteam/nopcommerce:4.90.8"
NOP_UID=0
NOP_GID=0
MYSQL_UID=999
MYSQL_GID=999

log()  { printf '\033[1;34m==>\033[0m %s\n' "$*"; }
ok()   { printf '\033[1;32m  ✔\033[0m %s\n' "$*"; }
warn() { printf '\033[1;33m警告:\033[0m %s\n' "$*" >&2; }
die()  { printf '\033[1;31mERROR:\033[0m %s\n' "$*" >&2; exit 1; }

compose() { docker compose -f "${COMPOSE_FILE}" "$@"; }

# ---------------------------------------------------------------- 依赖检查
command -v docker >/dev/null 2>&1 || die "未找到 docker"
docker info >/dev/null 2>&1 || die "无法访问 docker 守护进程
    未加 docker 组权限时: sg docker -c 'bash $0'"
docker compose version >/dev/null 2>&1 || die "未找到 docker compose 插件"

# ---------------------------------------------------------------- 子命令
case "${1:-}" in
  --status)
    compose ps
    exit 0 ;;
  --stop)
    compose stop
    exit 0 ;;
  --down)
    compose down
    ok "已删除容器与网络（data/ 数据保留）"
    exit 0 ;;
  --logs)
    compose logs -f
    exit 0 ;;
  -h|--help)
    sed -n '2,14p' "$0"
    exit 0 ;;
esac

# ---------------------------------------------------------------- 1. 确认镜像
for image in "${NOP_IMAGE}" "mysql:8.0"; do
  log "检查镜像 ${image} ..."
  if ! docker image inspect "${image}" >/dev/null 2>&1; then
    case "${image}" in
      "${NOP_IMAGE}") archive="nopcommerce.tar" ;;
      "mysql:8.0") archive="mysql.tar" ;;
    esac
    die "本机缺少镜像 ${image}。
    请先手动导入镜像: docker load -i ${SCRIPT_DIR}/${archive}
    导入完成后重新运行本脚本；部署过程不会联网下载镜像。"
  fi
done
ok "所需镜像均已就绪"

# ---------------------------------------------------------------- 2. 建目录
log "创建数据目录 ..."
for d in mysql nop-appdata nop-images nop-files nop-plugins nop-themes; do
  mkdir -p "${DATA_ROOT}/${d}"
done
ok "目录已创建于 ${DATA_ROOT}/"

# ---------------------------------------------------------------- 3. 修正属主
# MySQL 以 999:999 运行，nopCommerce 以 0:0 运行，
# 属主不一致会导致 "Permission denied" 或启动即崩。
log "修正属主（需要 sudo）..."
sudo chown -R "${MYSQL_UID}:${MYSQL_GID}" "${DATA_ROOT}/mysql"
sudo chmod -R 775 "${DATA_ROOT}/mysql"
sudo chown -R "${NOP_UID}:${NOP_GID}" \
  "${DATA_ROOT}/nop-appdata" "${DATA_ROOT}/nop-images" "${DATA_ROOT}/nop-files" \
  "${DATA_ROOT}/nop-plugins" "${DATA_ROOT}/nop-themes"
sudo chmod -R 775 \
  "${DATA_ROOT}/nop-appdata" "${DATA_ROOT}/nop-images" "${DATA_ROOT}/nop-files" \
  "${DATA_ROOT}/nop-plugins" "${DATA_ROOT}/nop-themes"
ok "属主已修正"

# ---------------------------------------------------------------- 4. 播种
# 宿主目录为空时，bind mount 会遮住镜像内的同路径目录。
# nopCommerce 启动时直接读 App_Data/ 并访问 wwwroot/images/uploaded/，
# 缺失会报 DirectoryNotFoundException，故必须先把镜像内容复制出来。
seed() {
  local host_dir="$1" container_path="$2"

  if [ -n "$(ls -A "${host_dir}" 2>/dev/null)" ]; then
    log "已有内容，跳过播种: ${host_dir}"
    return 0
  fi

  log "播种: ${container_path} -> ${host_dir}"
  local tmp base
  tmp="$(mktemp -d)"
  base="$(basename "${container_path}")"

  if docker run --rm --entrypoint tar "${NOP_IMAGE}" \
       -cf - -C "$(dirname "${container_path}")" "${base}" \
       | tar -xf - -C "${tmp}" \
     && [ -d "${tmp}/${base}" ]; then
    sudo cp -a "${tmp}/${base}/." "${host_dir}/"
    rm -rf "${tmp}"
  else
    rm -rf "${tmp}"
    die "播种 ${container_path} 失败，应用会因缺少目录而崩溃"
  fi
}

seed "${DATA_ROOT}/nop-appdata" "/app/App_Data"
seed "${DATA_ROOT}/nop-images"  "/app/wwwroot/images"
seed "${DATA_ROOT}/nop-files"   "/app/wwwroot/files"
seed "${DATA_ROOT}/nop-plugins" "/app/Plugins"
seed "${DATA_ROOT}/nop-themes"  "/app/Themes"

sudo chown -R "${NOP_UID}:${NOP_GID}" \
  "${DATA_ROOT}/nop-appdata" "${DATA_ROOT}/nop-images" "${DATA_ROOT}/nop-files" \
  "${DATA_ROOT}/nop-plugins" "${DATA_ROOT}/nop-themes"
ok "播种完成"

# ---------------------------------------------------------------- 5. 启动
log "启动服务 ..."
compose up -d

# ---------------------------------------------------------------- 6. 等待就绪
log "等待 MySQL 就绪 ..."
deadline=$(( SECONDS + 180 ))
state=unknown
while [ "${SECONDS}" -lt "${deadline}" ]; do
  cid="$(compose ps -q mysql 2>/dev/null || true)"
  if [ -n "${cid}" ]; then
    state="$(docker inspect -f '{{.State.Health.Status}}' "${cid}" 2>/dev/null || echo unknown)"
  fi
  case "${state}" in
    healthy) ok "MySQL 已就绪"; break ;;
    unhealthy)
      compose logs --tail 30 mysql
      die "MySQL 进入 unhealthy 状态" ;;
    *) printf '\r    当前状态: %s (%ss)' "${state}" "$((SECONDS))" ;;
  esac
  sleep 3
done

[ "${state}" = "healthy" ] || die "等待超时（180s）。
    排查: docker compose -f ${COMPOSE_FILE} logs mysql"

printf '\n'
compose ps

IP="$(hostname -I 2>/dev/null | awk '{print $1}')"
cat <<EOF

$(printf '\033[1;32m✔ 容器已启动\033[0m')

下一步 — 执行 nopCommerce 安装向导:
  http://${IP:-localhost}/

安装时填写:
  Data provider     MySQL
  Server name       mysql          ← 必须是容器名，不要用 localhost
  Database name     mymall
  MySQL user        nop
  Password          Mymall_N0p!Passw0rd
  Admin email       你想用的邮箱（请记下来）
  Admin password    你想用的密码（请记下来）

安装完成后验证后台可登录:
  http://${IP:-localhost}/admin

常用命令:
  状态    bash deploy.sh --status
  日志    bash deploy.sh --logs
  停止    bash deploy.sh --stop
  删除    bash deploy.sh --down
EOF
