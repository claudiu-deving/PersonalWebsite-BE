### What is this about?

Git CTS is a personal project meant for task tracking and management with a focus for the construction industry.

There are two main principles behind this idea, one being the incorporation of the ISO-EN 19650 and the other being the fact that task==folders.

Working in AEC industry I realized that while working on different tasks I would organically create different folders for each task with their own subfolders sometimes. This is how my idea started, leveraging the power of the git platform with a task management one, so one folder = one task and this would play well with the idea of the subfolder and subtasks.

The other principle, regarding the ISO 19650 has more to do with project management and it would be a gigantic task for me to undertake by my own since it's main focus would be the communication between the application with different APIs to create a single source of truth.

I will create this application in WPF since it will be mainly used together with a multitude of other software, most of them running only on Windows.

### Epics:

#### Epic 1: Kanban board

This will be focused on the core features for task management, backed by a Postgresql database.

The basic object will be a Task, represented by a card on the board, by pressing a button a model window will open giving the possibily to edit the task.The tasks will be placed into columns which in turn will be placed on the board. Columns will represent the status of the task To Do, In progress etc.

In total I have 3 objects, each with its own Service for CRUD operations and their UI representation.

.
