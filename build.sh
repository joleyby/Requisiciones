#!/usr/bin/env bash
# build.sh — Carga el esquema y datos de prueba en el contenedor sqlserver
set -euo pipefail

# ── Configuración ────────────────────────────────────────────────
CONTAINER="sqlserver"
SA_PASSWORD="Manuel@SQL!"
SQLCMD="/opt/mssql-tools18/bin/sqlcmd"
PROJECT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
DB_DIR="${PROJECT_DIR}/Database"

SCHEMA_FILE="${DB_DIR}/01_RequisicionesDB.sql"
SEED_FILE="${DB_DIR}/02_DB_datos_pruebas.sql"
REMOTE_PATH="/tmp/schema.sql"

# ── Verificaciones ───────────────────────────────────────────────
[[ -f "${SCHEMA_FILE}" ]] || { echo "❌ No existe: ${SCHEMA_FILE}"; exit 1; }
[[ -f "${SEED_FILE}"   ]] || { echo "❌ No existe: ${SEED_FILE}";   exit 1; }

docker inspect "${CONTAINER}" >/dev/null 2>&1 \
  || { echo "❌ El contenedor '${CONTAINER}' no está corriendo."; exit 1; }

# ── Helpers ──────────────────────────────────────────────────────
run_sql_file() {
  local local_file="$1"
  local label="$2"
  echo "▶ ${label}: $(basename "${local_file}")"
  docker cp "${local_file}" "${CONTAINER}:${REMOTE_PATH}"
  docker exec -i "${CONTAINER}" "${SQLCMD}" \
    -S localhost -U sa -P "${SA_PASSWORD}" -C -I \
    -i "${REMOTE_PATH}"
}

# ── Ejecución ────────────────────────────────────────────────────
echo "════════════════════════════════════════════════════════"
echo "  Build de base de datos — RequisicionesDB"
echo "════════════════════════════════════════════════════════"

run_sql_file "${SCHEMA_FILE}" "1/2 Esquema"
run_sql_file "${SEED_FILE}"   "2/2 Datos de prueba"

echo "════════════════════════════════════════════════════════"
echo "✅ Build completado"
echo "════════════════════════════════════════════════════════"
