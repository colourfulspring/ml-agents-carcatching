import os
import json
import numpy as np


def convert_to_npy(json_file_path, output_npy_path):
    # Load the JSON file
    with open(json_file_path, 'r') as f:
        json_data = json.load(f)

    # Extract 'step' and 'value' from the list of dictionaries
    steps = np.array([d['step'] for d in json_data])
    values = np.array([d['value'] for d in json_data])

    # Combine 'steps' and 'values' arrays into a single 2D array
    combined_array = np.array([steps, values])

    # Save the arrays into separate files
    np.save(output_npy_path, combined_array)


if __name__ == "__main__":

    # Source directory where files are located
    source_dir = os.path.join(os.getcwd(), "tensorboard_data_json_02_results")

    # Destination directory where files will be copied
    destination_dir = os.path.join(os.getcwd(), "tensorboard_data_numpy_03_results")

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

        # Define the new filename with '.npy' extension
        npy_filename = os.path.splitext(os.path.basename(file_path))[0] + '.npy'

        # Define the new path of .npy file
        npy_filepath = os.path.join(destination_dir, relative_dir, npy_filename)

        # Convert json to step.npy and value.npy
        convert_to_npy(file_path, npy_filepath)

        # Print out the coverted files
        print(f"Covert {file_path} to {npy_filepath}")

    print("Convert operation completed.")
        
