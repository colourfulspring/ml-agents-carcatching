import os
import numpy as np
import pandas as pd
import seaborn as sns
import matplotlib.pyplot as plt

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
    source_dir = os.path.join(os.getcwd(), "tensorboard_data_merged_numpy_04_results")

    # Destination directory where files will be copied
    destination_dir = os.path.join(os.getcwd(), "tensorboard_data_seaborn_05_results")

    # Find all files in the source directory starting with "events"
    files = []
    for root, dirs, filenames in os.walk(source_dir):
        for filename in filenames:
            if filename.startswith('events'):
                files.append(os.path.join(root, filename))

    # Legend labels
    legend_labels = {'v5.0.1': 'Pos+CNN', 'v5.1.0': 'Pos', 'v5.2.0': 'CNN'}

    # Create a line plot
    plt.figure(figsize=(10, 8))

    # Loop through each file found
    for num, file_path in enumerate(files):
        # Get the directory where the file is located relative to the source directory
        relative_dir = os.path.relpath(os.path.dirname(file_path), source_dir)

        # Split the directory specifying the version number, va.b.c-d
        first_level_dir = os.path.split(relative_dir.rstrip(os.sep))[0]

        # Split the version number to get va.b.c
        version_number = first_level_dir.split('-')[0]

        # Read npy file into array
        data = np.load(file_path)

        # Extract steps and values
        steps = data[0]
        values = data[1]

        # Smooth values
        smoothed_values = smooth(values, 0.95)

        

        plt.plot(steps, smoothed_values, linestyle='-', label=legend_labels[version_number])
        print(f"Add {file_path}")
    
    # Add labels and title
    plt.xlabel('Step')
    plt.ylabel('Cumulative Reward')
    plt.title('Line Plot of Step vs Cumulative Reward')

    # Show grid
    plt.grid(True)

    # Add legend
    plt.legend()

    # Create the corresponding directory structure in the destination directory
    os.makedirs(destination_dir, exist_ok=True)

    # Save the plot as a JPG file
    plt.savefig(os.path.join(destination_dir, 'line_plot.jpg'), format='jpg')

    print("plot operation completed.")
