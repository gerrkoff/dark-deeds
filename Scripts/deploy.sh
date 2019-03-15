 STATUS=$(git status)
 if [[ $STATUS != *"Your branch is up to date"* ]] || [[ $STATUS != *"nothing to commit, working tree clean"* ]]
 then
    echo Current branch is not clean
    exit 1
else
    echo Current branch is clean, move on
fi

git checkout staging
git pull origin staging
git merge origin/master
git push
git checkout master