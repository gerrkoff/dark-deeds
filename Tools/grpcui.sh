#!/usr/bin/env bash

# go get github.com/fullstorydev/grpcui/...
# go install github.com/fullstorydev/grpcui/cmd/grpcui

grpcui -insecure localhost:$1
# grpcui -plaintext localhost:12345