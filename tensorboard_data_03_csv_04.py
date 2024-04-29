import os
import json
import numpy as np
import pandas as pd


def smooth(scalars, weight):  # Weight between 0 and 1
    last = scalars[0]  # First value in the plot (first timestep)
    smoothed = list()
    for point in scalars:
        smoothed_val = last * weight + (1 - weight) * point  # Calculate smoothed value
        smoothed.append(smoothed_val)                        # Save it
        last = smoothed_val                                  # Anchor the last smoothed value

    return smoothed


if __name__ == "__main__":

    # Source directory where files are located
    source_dir = os.path.join(os.getcwd(), "tensorboard_data_02_json_results")

    # Destination directory where files will be copied
    destination_dir = os.path.join(os.getcwd(), "tensorboard_data_03_csv_results")

    # Find all files in the source directory starting with "events", choose 3 version to be draw on image
    files = []
    for root, dirs, filenames in os.walk(source_dir):
        normal_root = os.path.normpath(root)
        for filename in filenames:
            version = os.path.split(normal_root)[-2]
            if filename.startswith('events') and any(sub in version for sub in ('v8.0.0', 'v5.0.1')):
                files.append(os.path.join(root, filename))
    
    # Legend labels of tensorboard_data_03_csv.py
    legend_labels = {'v5.0.1': 'Decision Range', 'v8.0.0': 'No Decision Range'}

    # Create an empty DataFrame with three columns
    df = pd.DataFrame(columns=['Step', 'Value', 'Legend'])
    print(df)

    # Loop through each file found
    for file_path in files:
        # Get the directory where the file is located relative to the source directory
        relative_dir = os.path.relpath(os.path.dirname(file_path), source_dir)

        # Split the directory specifying the version number, va.b.c-d
        first_level_dir = os.path.split(relative_dir.rstrip(os.sep))[0]

        # Split the version number to get va.b.c
        version_number = first_level_dir.split('-')[0]

        # Load the JSON file
        with open(file_path, 'r') as f:
            json_data = json.load(f)

        # Extract 'step' and 'value' from the list of dictionaries
        steps = np.array([d['step'] for d in json_data])
        values = np.array([d['value'] for d in json_data])

        # Find smoothed values
        smoothed_values = smooth(values, 0.95)

        # Concatenate the two arrays along the second axis (columns)
        concatenated_array = np.stack((steps, smoothed_values), axis=1)
        print(concatenated_array.shape)

        # Create a DataFrame from the numpy array
        df2 = pd.DataFrame(concatenated_array, columns=['Step', 'Value'])

        # Concatenate the two DataFrames
        df = pd.concat([df, df2], ignore_index=True)

        # Assign the string legend_labels[version_number] to the last rows of the Legend column
        df.loc[len(df) - len(df2):, 'Legend'] = legend_labels[version_number]

        # Print out the coverted files
        print(f"Covert {file_path} to DataFrame")

    # Create the destination directory
    os.makedirs(destination_dir, exist_ok=True)

    # Define the new filename with '.csv'
    csv_filename = 'result_04.csv'

    # Define the new path of .csv file
    csv_filepath = os.path.join(destination_dir, csv_filename)

    # Write DataFrame to CSV file
    df.to_csv(csv_filepath, index=False)  # Set index=False to exclude the index column in the CSV file

    print("Convert operation completed.")
