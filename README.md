# search

to deploy the docker cluster:

```
$ mkdir -p /mnt/data && ELK_VERSION=6.6.0 docker-compose up -d
```

to test ingestion by logstash:

```
nc localhost 5000 < ./tests/logstash-tutorial.log
```

to query elastic search:

```
curl 'http://localhost:9200/_search?pretty'
```
