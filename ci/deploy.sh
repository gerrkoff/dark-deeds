#!/bin/bash
STATUS=$(LANG=en_US git status)
if [[ $STATUS != *"Your branch is up to date"* ]] || [[ $STATUS != *"nothing to commit, working tree clean"* ]]
then
    echo Current branch is not clean
    exit 1
else
    echo Current branch is clean, move on
fi

echo "Confirm merging to staging:"
read

git checkout master
git pull
git checkout staging
git pull
git merge master

git push
git checkout master

echo
echo "()___)____________)   Successfully merged to staging"
echo
