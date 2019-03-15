 STATUS=$(git status)
 if [ "$STATUS" != "On branch master Your branch is up to date with 'origin/master'. nothing to commit, working tree clean" ]
 then
    echo Current branch is not clean
    exit 1
else
    echo Cool
fi