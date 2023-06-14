# Elevator Project

This project is focused on implementing different types of elevators with varying functionalities. The current implementations include a Simple Elevator and a Smart Elevator.  
The project includes a console application demonstration and a small set of unit tests for the elevator logic of different elevator types.

## Elevator Types

### Simple Elevator

The Simple Elevator implementation fulfills elevator calls in a strict sequential order. It operates based on a first-come, first-served basis. When a call is received, the elevator will travel to the requested floor and serve the passenger. After completing the current request, it will proceed to the next call in the queue.

### Smart Elevator

The Smart Elevator is a modern elevator that optimizes calls by prioritizing those in the same direction as the currently executed request. This elevator aims to reduce waiting times by efficiently handling requests. Similar to the Simple Elevator, it operates on a first-come, first-served basis. The Smart Elevator will calculate the most efficient route based on the current direction and serve the passengers accordingly.

## Assumptions and Considerations

- The calculation assumes that it takes 1 period to travel between each floor.
- The calculation outcome is exposed via an IEnumerable which iterates over the visited floors. While idle, the IEnumerable infinitely generates the last visited floor, but the method can be configured to finish the enumeration when all calls were fulfilled.
- The time required for other calculations, such as queue management, is considered negligible.

## Simple Elevator Routine Description

1. The elevator starts in an idle state, waiting for a call.
2. When someone calls the elevator, their call is enqueued to a concurrent queue.
3. The calls are added to a queue based on their order of arrival.
4. The elevator dequeues a call and enqueues 2 floor destinations - the source and destination of the call.
5. After serving the passenger, the elevator proceeds to the next call in the queue if there are any.

## Project TODOs

- Organize classes into folders and namespaces
- Implement the smart elevator
- Change collection objects to interfaces
- Improve namings
- Add documentation in the code
- Implement validation for floors between 1 and the maximum height.
- Refactor the call function to accept only the origin and direction initially and return a promise allowing to enqueue target floor calls (more than 1 since several people could be waiting for the elevator).
- Implement a multi-elevator which has multiple elevators operating on a modern logic
- Add a statistical model for generating elevator use cases for benchmark purposes (chasing the lowest number of iterations serving all customers)
- Explore on the other modern optimizations of elevator functioning
- Move from 1 floor per iteration to arriving at a floor, generating a list of traversed floors