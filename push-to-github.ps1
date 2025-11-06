# Script to push Transport project to GitHub
# Usage: .\push-to-github.ps1 -RepoUrl "https://github.com/username/repo-name.git"

param(
    [Parameter(Mandatory=$true)]
    [string]$RepoUrl
)

Write-Host "=== Pushing Transport Project to GitHub ===" -ForegroundColor Cyan
Write-Host ""

# Check if remote already exists
$remoteExists = git remote get-url origin 2>$null
if ($remoteExists) {
    Write-Host "Remote 'origin' already exists: $remoteExists" -ForegroundColor Yellow
    $overwrite = Read-Host "Do you want to update it? (y/n)"
    if ($overwrite -eq "y" -or $overwrite -eq "Y") {
        git remote set-url origin $RepoUrl
        Write-Host "Updated remote origin to: $RepoUrl" -ForegroundColor Green
    } else {
        Write-Host "Keeping existing remote. Exiting." -ForegroundColor Yellow
        exit
    }
} else {
    git remote add origin $RepoUrl
    Write-Host "Added remote origin: $RepoUrl" -ForegroundColor Green
}

# Rename branch to main if needed
$currentBranch = git branch --show-current
if ($currentBranch -eq "master") {
    Write-Host "Renaming branch from 'master' to 'main'..." -ForegroundColor Yellow
    git branch -M main
    Write-Host "Branch renamed to 'main'" -ForegroundColor Green
}

# Push to GitHub
Write-Host ""
Write-Host "Pushing to GitHub..." -ForegroundColor Yellow
git push -u origin main

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "=== SUCCESS ===" -ForegroundColor Green
    Write-Host "Project has been pushed to GitHub successfully!" -ForegroundColor Green
    Write-Host "Repository URL: $RepoUrl" -ForegroundColor Cyan
} else {
    Write-Host ""
    Write-Host "=== ERROR ===" -ForegroundColor Red
    Write-Host "Failed to push to GitHub. Please check the error above." -ForegroundColor Red
}

