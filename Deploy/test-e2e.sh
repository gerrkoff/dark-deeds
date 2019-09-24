#!/usr/bin/env bash
dotnet test "--logger:trx;LogFileName=results.trx" --results-directory artifacts || exit $?
