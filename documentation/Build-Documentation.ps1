<#
.SYNOPSIS
	Converts the Markdown articles in this folder to HTML pages for the WIGG Directory application.

.DESCRIPTION
	Each .md file is converted with ConvertFrom-Markdown (PowerShell 7+) and wrapped with the same
	header and footer used by the application's MainLayout (see uofi-itp-directory\Shared\MainLayout.razor).
	Output goes to uofi-itp-directory\wwwroot\documentation and is served as static files
	(for example /documentation/index.html). README.md becomes index.html.
	Links between articles are rewritten from .md to .html.

	Run from any folder:  pwsh .\documentation\Build-Documentation.ps1
#>
[CmdletBinding()]
param(
	[string]$OutputPath = (Join-Path $PSScriptRoot '..\uofi-itp-directory\wwwroot\documentation')
)

$ErrorActionPreference = 'Stop'

$template = @'
<!DOCTYPE html>
<html lang="en">
<head>
	<meta charset="utf-8" />
	<meta name="viewport" content="width=device-width, initial-scale=1.0" />
	<title>{{TITLE}} | WIGG Directory Documentation | Illinois</title>
	<link rel="dns-prefetch" href="//cdn.toolkit.illinois.edu">
	<link rel="icon" href="https://cdn.brand.illinois.edu/favicon.ico">
	<link rel="stylesheet" href="https://cdn.toolkit.illinois.edu/3/toolkit.css">
	<script src="https://cdn.toolkit.illinois.edu/3/toolkit.js"></script>
	<script src="https://cdn.disability.illinois.edu/skipto.min.js"></script>
	<script>var SkipToConfig = { 'settings': { 'skipTo': { displayOption: 'popup', colorTheme: 'illinois' } } };</script>
	<link href="/css/site.css?cache=1" rel="stylesheet" />
</head>
<body>
<ilw-page>
	<ilw-header slot="header">
		<a slot="primary-unit" href="https://wigg.illinois.edu/">Web Implementation Guidelines Group</a>
		<a slot="site-name" href="/">WIGG Directory Application</a>
		<div slot="links">
			<a href="/">Back to the Application</a>
		</div>
	</ilw-header>
	<main>
		<ilw-content width="page" class="iled-section iled-section-full">
{{BACK}}
{{BODY}}
		</ilw-content>
	</main>
	<ilw-footer slot="footer">
		<a slot="primary-unit" href="https://wigg.illinois.edu/">Web Implementation Guidelines Group</a>
		<a slot="site-name" href="/">WIGG Directory Application</a>
		<div slot="actions"><a href="https://go.illinois.edu/directory-toolkit-help">Report a Problem</a></div>
	</ilw-footer>
</ilw-page>
</body>
</html>
'@

$utf8NoBom = New-Object System.Text.UTF8Encoding($false)
$null = New-Item -ItemType Directory -Force -Path $OutputPath
$OutputPath = (Resolve-Path $OutputPath).Path

# Remove previously generated pages so deleted articles do not linger.
Get-ChildItem -Path $OutputPath -Filter *.html -File | Remove-Item -Force

foreach ($file in Get-ChildItem -Path $PSScriptRoot -Filter *.md -File) {
	$isIndex = $file.Name -ieq 'README.md'
	$outName = if ($isIndex) { 'index.html' } else { [IO.Path]::ChangeExtension($file.Name, '.html') }

	$html = (ConvertFrom-Markdown -Path $file.FullName).Html

	# Rewrite links between articles (.md -> .html, README.md -> index.html).
	$html = [regex]::Replace($html, 'href="README\.md(#[^"]*)?"', 'href="index.html$1"')
	$html = [regex]::Replace($html, 'href="([^":/]+)\.md(#[^"]*)?"', 'href="$1.html$2"')

	# On the index page, drop the article numbers from the link text (the Markdown keeps them).
	if ($isIndex) {
		$html = [regex]::Replace($html, '(<a href="[^"]+">)\d+ - ', '$1')
	}

	# Open external links in the same tab (default), but make sure tables fit the layout.
	$html = $html -replace '<table>', '<div style="overflow-x: auto;"><table>' -replace '</table>', '</table></div>'

	$titleMatch = [regex]::Match($html, '<h1[^>]*>(.*?)</h1>', 'Singleline')
	$title = if ($titleMatch.Success) { [regex]::Replace($titleMatch.Groups[1].Value, '<[^>]+>', '') } else { 'Documentation' }

	$back = if ($isIndex) { '' } else { '            <p><a href="index.html">&larr; Back to Documentation Index</a></p>' }

	$page = $template.Replace('{{TITLE}}', $title).Replace('{{BACK}}', $back).Replace('{{BODY}}', $html)
	[IO.File]::WriteAllText((Join-Path $OutputPath $outName), $page, $utf8NoBom)
	Write-Host "Created $outName"
}
