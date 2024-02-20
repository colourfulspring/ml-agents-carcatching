import os
import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns


if __name__ == "__main__":

    # Source directory where files are located
    source_dir = os.path.join(os.getcwd(), "tensorboard_data_03_csv_results")

    # Destination directory where files will be copied
    destination_dir = os.path.join(os.getcwd(), "tensorboard_data_04_seaborn_results")

    # Find all files in the source directory starting with "events"
    files = []
    for root, dirs, filenames in os.walk(source_dir):
        normal_root = os.path.normpath(root)
        for filename in filenames:
            version = os.path.split(normal_root)[-2]
            files.append(os.path.join(root, filename))

    # 设置样式
    sns.set_theme(context='paper', style='darkgrid')

    # Loop through each file found
    for num, file_path in enumerate(files):
        # Create a line plot
        plt.figure(figsize=(10, 8))

        # Get the directory where the file is located relative to the source directory
        relative_dir = os.path.relpath(os.path.dirname(file_path), source_dir)

        # Split the directory specifying the version number, va.b.c-d
        first_level_dir = os.path.split(relative_dir.rstrip(os.sep))[0]

        # Split the version number to get va.b.c
        version_number = first_level_dir.split('-')[0]

        # Read the CSV file into a pandas DataFrame
        df = pd.read_csv(file_path)

        # Create the corresponding directory structure in the destination directory
        os.makedirs(os.path.join(destination_dir, relative_dir), exist_ok=True)

        sns.lineplot(x="Step", y="Value", data=df, hue="Legend", errorbar=('sd', 1))

        # Define the new filename with '.jpg' extension
        jpg_filename = os.path.splitext(os.path.basename(file_path))[0] + '.jpg'

        # Add labels and title
        plt.xlabel('Step')
        plt.ylabel('Cumulative Reward')
        plt.title('Line Plot of Step vs Cumulative Reward')

        # Show grid
        plt.grid(True)

        # Add legend
        plt.legend()

        # Save the plot as a JPG file
        plt.savefig(os.path.join(destination_dir, jpg_filename), format='jpg')

        print(f"Plot {file_path}")

    print("Plot operation completed.")
