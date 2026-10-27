#!/bin/bash
#
# Cadastra veículos de teste na API local.
# Requer: macOS, bash e curl.
#
# Uso:
#   chmod +x seed-vehicles.sh
#   ./seed-vehicles.sh
#
# Opcional:
#   API_URL="http://localhost:5180/api/v1/vehicles" ./seed-vehicles.sh
#
# ATENÇÃO: os dados são fictícios para teste. As marcas/modelos existem no Brasil,
# mas placas e combinações de ano/preço não representam veículos reais específicos.
# Cada execução tentará cadastrar novamente os mesmos 55 registros.

set -u

API_URL="${API_URL:-http://localhost:5180/api/v1/vehicles}"
SUCCESS=0
FAILED=0
TOTAL=0

while IFS='|' read -r brand model plate year color price; do
  [[ -z "${brand}" ]] && continue
  TOTAL=$((TOTAL + 1))

  body=$(printf '{"brand":"%s","model":"%s","licensePlate":"%s","year":%s,"color":"%s","price":%s}' \
    "$brand" "$model" "$plate" "$year" "$color" "$price")

  http_code=$(curl --silent --show-error --location \
    --output /tmp/seed-vehicle-response.json \
    --write-out '%{http_code}' \
    --request POST "$API_URL" \
    --header 'Content-Type: application/json' \
    --data "$body")
  curl_status=$?

  if [[ "$curl_status" -eq 0 && "$http_code" -ge 200 && "$http_code" -lt 300 ]]; then
    SUCCESS=$((SUCCESS + 1))
    printf '[%02d/%02d] OK   %s %s (%s) - HTTP %s\n' "$TOTAL" 55 "$brand" "$model" "$plate" "$http_code"
  else
    FAILED=$((FAILED + 1))
    printf '[%02d/%02d] ERRO %s %s (%s) - HTTP %s\n' "$TOTAL" 55 "$brand" "$model" "$plate" "${http_code:-curl-error}"
    if [[ -f /tmp/seed-vehicle-response.json ]]; then
      cat /tmp/seed-vehicle-response.json
      printf '\n'
    fi
  fi
done <<'VEHICLES'
Fiat|Strada|BRA1A01|2024|Branco|112900
Volkswagen|Polo|BRA1A02|2023|Prata|87900
Fiat|Argo|BRA1A03|2022|Cinza|73900
Volkswagen|T-Cross|BRA1A04|2024|Preto|142900
Volkswagen|Tera|BRA1A05|2026|Branco|119900
Chevrolet|Onix|BRA1A06|2023|Vermelho|82900
BYD|Dolphin Mini|BRA1A07|2025|Verde|119800
Hyundai|Creta|BRA1A08|2022|Prata|109900
Hyundai|HB20|BRA1A09|2021|Branco|68900
BYD|Song Plus|BRA1A10|2025|Cinza|239800
Fiat|Mobi|BRA1A11|2022|Vermelho|59900
Renault|Kwid|BRA1A12|2023|Branco|62900
Chevrolet|Tracker|BRA1A13|2024|Azul|129900
Fiat|Toro|BRA1A14|2023|Prata|139900
Fiat|Pulse|BRA1A15|2024|Preto|109900
Jeep|Compass|BRA1A16|2022|Cinza|149900
Volkswagen|Nivus|BRA1A17|2023|Azul|119900
Toyota|Corolla Cross|BRA1A18|2024|Branco|169900
Toyota|Corolla|BRA1A19|2020|Prata|150000
Toyota|Hilux|BRA1A20|2022|Preto|229900
Honda|HR-V|BRA1A21|2023|Cinza|149900
Jeep|Renegade|BRA1A22|2021|Verde|99900
Hyundai|HB20S|BRA1A23|2022|Prata|79900
Chevrolet|Onix Plus|BRA1A24|2024|Branco|99900
Volkswagen|Saveiro|BRA1A25|2022|Branco|89900
Nissan|Kicks|BRA1A26|2023|Vermelho|109900
Fiat|Fastback|BRA1A27|2024|Cinza|129900
BYD|Dolphin|BRA1A28|2024|Azul|149800
Chevrolet|S10|BRA1A29|2021|Prata|179900
Ford|Ranger|BRA1A30|2023|Preto|239900
Caoa Chery|Tiggo 5X|BRA1A31|2022|Branco|99900
Caoa Chery|Tiggo 7|BRA1A32|2024|Cinza|149900
Volkswagen|Virtus|BRA1A33|2023|Prata|109900
Fiat|Cronos|BRA1A34|2022|Branco|78900
Honda|City Sedan|BRA1A35|2024|Azul|119900
Honda|City Hatchback|BRA1A36|2023|Cinza|109900
Chevrolet|Montana|BRA1A37|2024|Preto|129900
Fiat|Fiorino|BRA1A38|2022|Branco|99900
Jeep|Commander|BRA1A39|2023|Preto|189900
GWM|Haval H6|BRA1A40|2024|Cinza|189900
BYD|Song Pro|BRA1A41|2025|Prata|189800
BYD|King|BRA1A42|2025|Branco|175800
Renault|Kardian|BRA1A43|2025|Laranja|119900
Citroën|C3|BRA1A44|2023|Branco|69900
Ram|Rampage|BRA1A45|2024|Preto|219900
Chevrolet|Spin|BRA1A46|2022|Prata|99900
Volkswagen|Gol|BRA1A47|2019|Branco|49900
Volkswagen|Virtus|BRA1A48|2021|Cinza|89900
Toyota|Yaris|BRA1A49|2020|Prata|79900
Mitsubishi|L200 Triton|BRA1A50|2022|Preto|189900
Peugeot|208|BRA1A51|2024|Amarelo|89900
Nissan|Versa|BRA1A52|2022|Azul|84900
Volkswagen|Taos|BRA1A53|2023|Cinza|149900
Toyota|SW4|BRA1A54|2021|Branco|289900
Volvo|XC40|BRA1A55|2023|Azul|229900
VEHICLES

printf '\nResumo: %s tentativas | %s sucesso(s) | %s falha(s)\n' "$TOTAL" "$SUCCESS" "$FAILED"
rm -f /tmp/seed-vehicle-response.json

if [[ "$FAILED" -gt 0 ]]; then
  exit 1
fi
exit 0
