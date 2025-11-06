# Auto-push script - Waits for repository to be created, then pushes
param(
    [Parameter(Mandatory=$false)]
    [string]$RepoName = "transport-management",
    [Parameter(Mandatory=$false)]
    [int]$MaxAttempts = 30,
    [Parameter(Mandatory=$false)]
    [int]$WaitSeconds = 5
)

$username = "DevPuPuBug"
$repoUrl = "https://github.com/$username/$RepoName.git"

Write-Host "=== AUTO-PUSH TO GITHUB ===" -ForegroundColor Cyan
Write-Host "Repository: $repoUrl" -ForegroundColor Yellow
Write-Host ""

# Update remote
git remote set-url origin $repoUrl
Write-Host "Remote configured" -ForegroundColor Green

# Ensure branch is main
git branch -M main
Write-Host "Branch set to main" -ForegroundColor Green

Write-Host ""
Write-Host "Waiting for repository to be created on GitHub..." -ForegroundColor Yellow
Write-Host "Please create repository at: https://github.com/new" -ForegroundColor Cyan
Write-Host "Repository name: $RepoName" -ForegroundColor White
Write-Host "DO NOT initialize with README" -ForegroundColor Yellow
Write-Host ""

$attempt = 0
$success = $false

while ($attempt -lt $MaxAttempts -and -not $success) {
    $attempt++
    Write-Host "Attempt $attempt/$MaxAttempts - Trying to push..." -ForegroundColor Yellow
    
    $output = git push -u origin main 2>&1 | Out-String
    
    if ($LASTEXITCODE -eq 0) {
        $success = $true
        Write-Host ""
        Write-Host "=== SUCCESS ===" -ForegroundColor Green
        Write-Host "Code pushed successfully!" -ForegroundColor Green
        Write-Host "Repository: $repoUrl" -ForegroundColor Cyan
        $webUrl = $repoUrl -replace '\.git$',''
        Write-Host "View on GitHub: $webUrl" -ForegroundColor White
        break
    } elseif ($output -match "Repository not found") {
        Write-Host "  Repository not found yet. Waiting..." -ForegroundColor Yellow
        if ($attempt -lt $MaxAttempts) {
            Start-Sleep -Seconds $WaitSeconds
        }
    } elseif ($output -match "Authentication failed" -or $output -match "Permission denied") {
        Write-Host ""
        Write-Host "=== AUTHENTICATION ERROR ===" -ForegroundColor Red
        Write-Host "Please check your GitHub credentials" -ForegroundColor Yellow
        Write-Host "You may need to:" -ForegroundColor Yellow
        Write-Host "1. Set up GitHub credentials in Git" -ForegroundColor White
        Write-Host "2. Or use Personal Access Token" -ForegroundColor White
        break
    } else {
        Write-Host "  Error: $output" -ForegroundColor Red
        break
    }
}

if (-not $success) {
    Write-Host ""
    Write-Host "=== TIMEOUT ===" -ForegroundColor Red
    Write-Host "Repository not found after $MaxAttempts attempts." -ForegroundColor Yellow
    Write-Host "Please ensure:" -ForegroundColor Yellow
    Write-Host "1. Repository is created at: https://github.com/$username/$RepoName" -ForegroundColor White
    Write-Host "2. Repository name matches: $RepoName" -ForegroundColor White
    Write-Host "3. You have push access" -ForegroundColor White
    Write-Host ""
    Write-Host "Then run manually:" -ForegroundColor Yellow
    Write-Host "  git push -u origin main" -ForegroundColor White
}

