#!/bin/bash

# Source directory where files are located
source_dir="./results"

# Destination directory where files will be copied
destination_dir="./tensorboard_results"

# Find all files in the source directory starting with "events"
files=$(find "$source_dir" -type f -name 'events*')

# Loop through each file found
for file in $files; do
    # Get the directory where the file is located relative to the source directory
    relative_dir=$(dirname "${file#$source_dir}")

    # Create the corresponding directory structure in the destination directory
    mkdir -p "$destination_dir/$relative_dir"

    # Copy the file to the destination directory, preserving the directory structure
    cp "$file" "$destination_dir/$relative_dir"
    
    # Print out the copied files
    echo "Copied $file to $destination_dir/$relative_dir"
done

echo "Copy operation completed."

