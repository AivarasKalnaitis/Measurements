# Measurement Sampler

## Overview

This repository contains a C# project for sampling measurement data over specific intervals. The project includes classes to manage measurements and intervals, as well as unit tests to ensure the functionality of the sampling logic.

## Project Structure

- **Measurements**: Contains the core logic for sampling measurements.
- **Measurements.Tests**: Contains unit tests for the sampling logic using the xUnit testing framework.

## Installation

1. Clone the repository:
    ```sh
    git clone https://github.com/yourusername/measurements-sampler.git
    cd measurements-sampler
    ```

2. Ensure you have the .NET SDK installed. You can download it from [here](https://dotnet.microsoft.com/download).

3. Restore the dependencies:
    ```sh
    dotnet restore
    ```

## Running Tests

To run the unit tests, navigate to the `Measurements.Tests` directory and use the following command:
```sh
dotnet test
