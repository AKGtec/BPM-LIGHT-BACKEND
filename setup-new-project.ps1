param(
    [Parameter(Mandatory=$true)]
    [string]$ProjectName
)

Write-Host "Setting up new project: $ProjectName" -ForegroundColor Green

# Validate project name
if ($ProjectName -match '[^a-zA-Z0-9._]') {
    Write-Error "Project name can only contain letters, numbers, dots, and underscores"
    exit 1
}

# Get current directory
$currentDir = Get-Location

# Function to replace content in files
function Replace-ContentInFile {
    param($FilePath, $OldText, $NewText)
    
    if (Test-Path $FilePath) {
        $content = Get-Content $FilePath -Raw
        $newContent = $content -replace $OldText, $NewText
        Set-Content $FilePath $newContent -NoNewline
        Write-Host "Updated: $FilePath" -ForegroundColor Yellow
    }
}

# Function to rename files and directories
function Rename-ProjectItems {
    param($Path, $OldName, $NewName)
    
    Get-ChildItem $Path -Recurse | Where-Object { $_.Name -like "*$OldName*" } | ForEach-Object {
        $newName = $_.Name -replace $OldName, $NewName
        $newPath = Join-Path $_.Directory $newName
        Rename-Item $_.FullName $newPath
        Write-Host "Renamed: $($_.Name) -> $newName" -ForegroundColor Cyan
    }
}

Write-Host "Step 1: Renaming directories and files..." -ForegroundColor Blue

# Rename directories first (from deepest to shallowest)
$directories = Get-ChildItem $currentDir -Directory -Recurse | Where-Object { $_.Name -like "*ProjectTemplate*" } | Sort-Object FullName -Descending
foreach ($dir in $directories) {
    $newName = $dir.Name -replace "ProjectTemplate", $ProjectName
    $newPath = Join-Path $dir.Parent.FullName $newName
    Rename-Item $dir.FullName $newPath
    Write-Host "Renamed directory: $($dir.Name) -> $newName" -ForegroundColor Cyan
}

# Rename files
$files = Get-ChildItem $currentDir -File -Recurse | Where-Object { $_.Name -like "*ProjectTemplate*" }
foreach ($file in $files) {
    $newName = $file.Name -replace "ProjectTemplate", $ProjectName
    $newPath = Join-Path $file.Directory $newName
    Rename-Item $file.FullName $newPath
    Write-Host "Renamed file: $($file.Name) -> $newName" -ForegroundColor Cyan
}

Write-Host "Step 2: Updating file contents..." -ForegroundColor Blue

# Get all text files to update content
$textFiles = Get-ChildItem $currentDir -Recurse -Include "*.cs", "*.csproj", "*.sln", "*.json", "*.md", "*.config" | Where-Object { !$_.PSIsContainer }

foreach ($file in $textFiles) {
    Replace-ContentInFile $file.FullName "ProjectTemplate" $ProjectName
    Replace-ContentInFile $file.FullName "projecttemplate" $ProjectName.ToLower()
    Replace-ContentInFile $file.FullName "PROJECTTEMPLATE" $ProjectName.ToUpper()
}

Write-Host "Step 3: Updating database context and connection string..." -ForegroundColor Blue

# Update database name in appsettings
$appSettingsFiles = Get-ChildItem $currentDir -Recurse -Name "appsettings*.json"
foreach ($settingsFile in $appSettingsFiles) {
    $fullPath = Join-Path $currentDir $settingsFile
    Replace-ContentInFile $fullPath "ProjectTemplateDB" "$($ProjectName)DB"
}

Write-Host "Setup completed successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Update the connection string in appsettings.json with your database server details"
Write-Host "2. Update JWT secret key in appsettings.json"
Write-Host "3. Run 'dotnet restore' to restore packages"
Write-Host "4. Run 'dotnet ef database update --project $ProjectName.API' to create the database"
Write-Host "5. Run 'dotnet run --project $ProjectName.API' to start the application"
Write-Host ""
Write-Host "Happy coding! 🚀" -ForegroundColor Green
