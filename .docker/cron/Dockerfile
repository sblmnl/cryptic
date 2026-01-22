FROM alpine:3.19

RUN apk update \
  && apk add --no-cache curl postgresql-client

RUN addgroup -S app \
  && adduser -S app -G app

ENV SUPERCRONIC_URL=https://github.com/aptible/supercronic/releases/download/v0.2.41/supercronic-linux-amd64 \
    SUPERCRONIC_SHA256SUM=798d0f6cf11cb74109b6408c50b1222cdd7678e8e70895dcfa9c2701b4bd03d5 \
    SUPERCRONIC=supercronic-linux-amd64

RUN curl -fsSLO "$SUPERCRONIC_URL" \
  && echo "${SUPERCRONIC_SHA256SUM}  ${SUPERCRONIC}" | sha256sum -c - \
  && chmod +x "$SUPERCRONIC" \
  && mv "$SUPERCRONIC" "/usr/local/bin/${SUPERCRONIC}" \
  && ln -s "/usr/local/bin/${SUPERCRONIC}" /usr/local/bin/supercronic

COPY --chown=app:app crontab /app/crontab
COPY --chown=app:app scripts/ /app/scripts/

USER app

CMD ["supercronic", "/app/crontab"]
