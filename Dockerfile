ARG ELK_VERSION

FROM docker.elastic.co/logstash/logstash-oss:${ELK_VERSION}

# RUN logstash-plugin install logstash-filter-json
