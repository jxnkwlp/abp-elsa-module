get-childitem -r "*/bin" -Exclude "*/node_module/*" | foreach ($_) { echo $_.FullName; remove-item $_ -r }
get-childitem -r "*/obj" -Exclude "*/node_modules/*" | foreach ($_) { echo $_.FullName; remove-item $_ -r }
