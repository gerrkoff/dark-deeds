 STATUS=$(git status)
 if [[ $STATUS != *"Your branch is up to date"* ]] || [[ $STATUS != *"nothing to commit, working tree clean"* ]]
 then
    echo Current branch is not clean
    exit 1
else
    echo Cool
fi