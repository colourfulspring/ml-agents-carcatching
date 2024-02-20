import os
import json
from tensorboard.backend.event_processing.event_accumulator import EventAccumulator, ScalarEvent

def scalar_event_list_to_dict_list(scalar_event_list):
    dict_list = []
    for scalar_event in scalar_event_list:
        # remove wall_time
        dict_list.append({'step': scalar_event.step, 'value': scalar_event.value})
    return dict_list


def convert_to_json(events_file_path, output_json_path):
    accumulator = EventAccumulator(events_file_path)
    accumulator.Reload()  # loads events from the file

    # Get a List[ScalarEvents]
    scalar_events = accumulator.Scalars('Environment/Cumulative Reward')

    # Convert the List[ScalarEvents] to List[Dict]
    dict_list = scalar_event_list_to_dict_list(scalar_events)

    # Dump json from List[Dict], then write JSON to a file
    with open(output_json_path, "w") as json_file:
        json.dump(dict_list, json_file)


if __name__ == "__main__":
   
    # Source directory where files are located
    source_dir = os.path.join(os.getcwd(), "tensorboard_data_copy_01_results")

    # Destination directory where files will be copied
    destination_dir = os.path.join(os.getcwd(), "tensorboard_data_json_02_results")

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

        # Define the new filename with '.json' extension
        json_filename = os.path.splitext(os.path.basename(file_path))[0] + '.json'

        # Define the new path of .json file
        json_filepath = os.path.join(destination_dir, relative_dir, json_filename)

        # Convert tensorboard events file to json
        convert_to_json(file_path, json_filepath)

        # Print out the coverted files
        print(f"Covert {file_path} to {json_filepath}")

    print("Convert operation completed.")
        
