 STATUS=$(git status)
 if [[ $STATUS != *"Your branch is up to date"* ]] || [[ $STATUS != *"nothing to commit, working tree clean"* ]]
 then
    echo Current branch is not clean
    exit 1
else
    echo Current branch is clean, move on
fi

git checkout master
git pull
git checkout staging
git pull
git merge master

sh bump-version.sh
git commit -m 'bump version' ../DarkDeeds/DarkDeeds.Api/DarkDeeds.Api.csproj

git push
git checkout master
git merge staging
git push

echo
echo "()___)____________)   Successfully merged to staging"
echo