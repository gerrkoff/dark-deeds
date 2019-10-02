#!/usr/bin/env bash
current_date_time="`date`";
echo $current_date_time;

test="`cat /etc/timezone`"
echo $test

dotnet test "--logger:trx;LogFileName=results.trx" --results-directory artifacts || exit $?
