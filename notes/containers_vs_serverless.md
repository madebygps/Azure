# Containers vs Serverless

## What is a container?

A container is a lightweight, stand-alone, executable package of a piece of software that includes everything needed to run it, including code, runtime, system tools, system libraries, settings, etc. By containerizing the application and its dependencies, differences in OS distributions and underlying infrastructure are abstracted away.

## Container Pros & Cons

- Using containers means you won't have any auto-scaling by default.
- They introduce complexity. You need to learn about the ecosystem and the various tools at your disposal.
- There are many monitoring and debugging tools you have at your disposal.
- Your team will have the same development environment no matter which operating system they're using.
- With little to no fuss, you can refactor an existing monolithic applications to container-based setups.

## Container use cases

The use-cases for containerized applications are significantly wider than with serverless. Whatever you already use traditional servers for would be a great candidate to be put into a container

## What is serverless?

An event based system for running code. You use various services to create business logic without caring about any servers. You're abstracting away the infrastructure altogether.

## Serverless Pros & Cons

- There are no operating system updates to install, no security patches, no worries, because the provider handles it for you.

- You will have to live with defined limits for processing power and memory, pushing you to write more efficient code because of the risk to overload your functions.

- FaaS solutions suffer from what are called cold-starts. The initial invocation of a function will take around a second or two for the container spin up. If this is a problem, you should reconsider using FaaS.

- No Dockerfiles or Kubernetes configurations. Your time-to-market will be amazing, something startups value more than anything else.

- The ephemeral nature of serverless functions makes them ideal for processing data streams or images.

## Serverless use cases

- Microservice architectures. 
- You can also use them as Cron jobs where you schedule a function to run at a specific time every day.
