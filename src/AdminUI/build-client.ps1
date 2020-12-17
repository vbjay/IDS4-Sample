pushd $PSScriptRoot\.\src\AdminUI.Admin
npm clean-install
gulp clean
gulp
pushd ..\AdminUI.STS.Identity
npm clean-install
gulp clean
gulp
popd
popd
