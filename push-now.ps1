# Quick push script - Run this after creating repository on GitHub
param(
    [Parameter(Mandatory=$false)]
    [string]$RepoName = "transport-management"
)

$username = "luuDevBiker"
$repoUrl = "https://github.com/$username/$RepoName.git"

Write-Host "=== PUSHING TO GITHUB ===" -ForegroundColor Cyan
Write-Host "Repository: $repoUrl" -ForegroundColor Yellow
Write-Host ""

# Update remote
git remote set-url origin $repoUrl
Write-Host "Remote updated" -ForegroundColor Green

# Ensure branch is main
git branch -M main
Write-Host "Branch set to main" -ForegroundColor Green

# Push
Write-Host "Pushing to GitHub..." -ForegroundColor Yellow
git push -u origin main

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "=== SUCCESS ===" -ForegroundColor Green
    Write-Host "Repository pushed successfully!" -ForegroundColor Green
    Write-Host "View at: $repoUrl" -ForegroundColor Cyan
    $webUrl = $repoUrl -replace '\.git$',''
    Write-Host "Web URL: $webUrl" -ForegroundColor White
} else {
    Write-Host ""
    Write-Host "=== ERROR ===" -ForegroundColor Red
    Write-Host "Failed to push. Please ensure:" -ForegroundColor Yellow
    Write-Host "1. Repository exists on GitHub: https://github.com/$username/$RepoName" -ForegroundColor White
    Write-Host "2. You have push access" -ForegroundColor White
    Write-Host "3. Repository is not initialized with README" -ForegroundColor White
}

