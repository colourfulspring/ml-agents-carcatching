import os
import pandas as pd
import matplotlib
import matplotlib.pyplot as plt
import seaborn as sns


if __name__ == "__main__":

    # fname 为 你下载的字体库路径，注意 SourceHanSansSC-Regular.otf 字体的路径
    zhfont1 = matplotlib.font_manager.FontProperties(fname="msyhbd.ttc") 

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
    sns.set_theme(context='paper', style="whitegrid", font_scale=2, rc={"font.weight": "bold"})

    # Loop through each file found
    for num, file_path in enumerate(files):
        # Create a line plot
        plt.figure(figsize=(10, 6))

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

        sns.lineplot(x="Step", y="Value", data=df, hue="Legend", errorbar='sd')

        # Define the new filename with '.pdf' extension
        pdf_filename = os.path.splitext(os.path.basename(file_path))[0] + '.pdf'

        # Add labels and title
        plt.xlabel('训练步数', fontproperties=zhfont1, fontsize=17)
        plt.ylabel('平均回报', fontproperties=zhfont1, fontsize=17)

        # Show grid
        plt.grid(True)

        # Add legend
        plt.legend()

        # Save the plot as a pdf file
        plt.savefig(os.path.join(destination_dir, pdf_filename), format='pdf', dpi=300, bbox_inches='tight')

        print(f"Plot {file_path}")

    print("Plot operation completed.")
