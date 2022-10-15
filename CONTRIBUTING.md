# Contributing to Dotnet Timecode

### How to request features or bugfixes

Any feature or bug report can be requested through the use of an issue in the Issues tab. Describe the feature or bug in detail and provide examples if possible. If you would like to implement the change yourself, ask to be assigned to the issue in the issue comment section to avoid that multiple people work on the same issue.

It is recommended to wait for confirmation from a code collaborator regarding if an issue should be implemented or not before starting any work, so that a discussion regarding the issue can be held in the issue comment section.

### Github workflow

- [Fork](https://docs.github.com/en/get-started/quickstart/fork-a-repo) the repo to add a copy of the project to your Github.

- Clone the repo to a local directory: `git clone https://github.com/<enter your github username here>/dotnet-timecode.git`

- Implement your changes to the project codebase.

- Add your changes in the working directory to the staging area: `git add -A`

- Commit your changes: `git commit -m "<add your message here>"`

- Push your changes to your github fork: `git push`

- Open a pull request from the forked repository github page into the original repository.

### How to write commit messages.

Shortly describe what was changed in the commit. For consistency it is preferred that messages are written using Imperative Mood. When in doubt, imagine that the commit message should continue the sentence "If applied, this commit will ...", where the ... represents the commit message.

Ex: git commit -m "Add feature x to y."

### How to handle pull requests

Describe what the submitted pull request aims to implement. A code collaborator must review the changes and approve the pull request for test workflows to run. All requested changes made by a reviewer must be resolved and all code tests must pass before the pull request will accepted and be merged with the main repository.

### Naming conventions

Follow the standard naming conventions for C# provided by the official .NET documentation.

### Tests

All code alterations must be covered by tests. Pull requests submitting code changes without test coverage will not be accepted.

### Code comments

All public classes, interfaces, properties and methods must have code comments describing the endpoints in detail. Private or internal endpoints do not require code comments unless requested by a collaborator, but descriptive comments are always appreciated as it increases code readability.

### Lastly

Happy coding!
