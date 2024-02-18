import os
import shutil

# Source directory where files are located
source_dir = "./results"

# Destination directory where files will be copied
destination_dir = "./tensorboard_results"

# Find all files in the source directory starting with "events"
files = []
for root, dirs, filenames in os.walk(source_dir):
    for filename in filenames:
        if filename.startswith('events'):
            files.append(os.path.join(root, filename))

# Loop through each file found
for file_path in files:
    # Get the directory where the file is located relative to the source directory
    relative_dir = os.path.relpath(os.path.dirname(file_path), source_dir)

    # Create the corresponding directory structure in the destination directory
    os.makedirs(os.path.join(destination_dir, relative_dir), exist_ok=True)

    # Copy the file to the destination directory, preserving the directory structure
    shutil.copy(file_path, os.path.join(destination_dir, relative_dir))

    # Print out the copied files
    print(f"Copied {file_path} to {os.path.join(destination_dir, relative_dir)}")

print("Copy operation completed.")
