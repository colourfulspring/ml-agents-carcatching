import os
import numpy as np


if __name__ == "__main__":

    # Source directory where files are located
    source_dir = os.path.join(os.getcwd(), "tensorboard_data_numpy_03_results")

    # Destination directory where files will be copied
    destination_dir = os.path.join(os.getcwd(), "tensorboard_data_merged_numpy_04_results")

    # Find all files in the source directory starting with "events"
    files = []
    for root, dirs, filenames in os.walk(source_dir):
        normal_root = os.path.normpath(root)
        for filename in filenames:
            version = os.path.split(normal_root)[-2]
            if filename.startswith('events') and any(sub in version for sub in ('v5.0.1', 'v5.1.0', 'v5.2.0')) :
                files.append(os.path.join(root, filename))

    values = 0

    # Loop through each file found
    for num, file_path in enumerate(files):
        # Get the directory where the file is located relative to the source directory
        relative_dir = os.path.relpath(os.path.dirname(file_path), source_dir)

        # Split the directory specifying the version number, va.b.c-d
        first_level_dir = os.path.split(relative_dir.rstrip(os.sep))[0]

        # Split the version number to get va.b.c
        version_number = first_level_dir.split('-')[0]

        # Load the data
        data = np.load(file_path)

        # Extract steps and values
        steps = data[0]
        values = values + data[1]

        # Merge results from many seeds
        if num % 3 == 2:
            values = values / 3

            # Create the corresponding directory structure in the destination directory
            os.makedirs(os.path.join(destination_dir, relative_dir), exist_ok=True)

            # Define the new filename with '.npy' extension
            npy_filename = os.path.splitext(os.path.basename(file_path))[0] + '.npy'

            # Define the new path of .npy file
            npy_filepath = os.path.join(destination_dir, relative_dir, npy_filename)

            # Convert 'steps' and 'values' arrays into a numpy array
            combined_array = np.array([steps, values])

            # Save the Dataframe into npy files
            np.save(npy_filepath, combined_array)

            print(f"Save merged pandas Dataframe {version_number}")

        print(f"Add {file_path}")


    print("Convert operation completed.")
