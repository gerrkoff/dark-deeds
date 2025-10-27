---
applyTo: '**'
---
# Dark Deeds Project Overview

This document provides a comprehensive overview of the Dark Deeds project - its purpose, domain concepts, and key features.

## Project Purpose

Dark Deeds is a **task management application** (todo list) designed for personal productivity. It focuses on organizing tasks across days with a calendar-based interface, allowing users to plan and manage their daily activities efficiently.

The application emphasizes:
- Visual task organization by date
- Flexible task types for different use cases
- Recurring task patterns
- Focused time horizon (current and next week)

## Core Concepts

### Tasks

Tasks are the fundamental unit of work in the system. Each task has:
- **Title**: Description of what needs to be done
- **Date**: When the task is scheduled (optional - tasks can have no date)
- **Time**: Specific time of day for the task (optional)
- **Completed**: Whether the task is done
- **Type**: Visual/organizational classification (see Task Types below)
- **Order**: Position within a day for drag-and-drop organization
- **isProbable**: Marks tasks that might not happen (displayed differently)

### Task Types

Tasks can be one of four types, which affect their visual appearance and organization:

1. **Simple** (`TaskType.Simple`): Default task type for regular todos
2. **Additional** (`TaskType.Additional`): Secondary or bonus tasks
3. **Routine** (`TaskType.Routine`): Regular maintenance or habitual tasks
4. **Weekly** (`TaskType.Weekly`): Tasks that happen once per week

These types are primarily visual indicators and help users organize tasks by importance or category.

### Date Organization

The application has a unique approach to date display:

- **Visible Period**: Tasks are shown for the **current week and next week** (14 days total)
- **Smart Day Display**: Days are only displayed if they contain tasks
- **No Date Section**: Tasks without a specific date are shown in a separate "No date" section
- **Week Blocks**: Days are organized into weekly blocks (Current, Next)

This focused time horizon keeps users centered on immediate and near-future tasks without overwhelming them with long-term planning.

### Recurring Tasks (Recurrences)

Recurrences allow users to define task patterns that repeat according to a schedule. This is a two-step process:

1. **Configuration**: In the "Recurrences" tab, users define:
   - Task template (what task to create)
   - Schedule pattern:
     - Every N days (`everyNthDay`)
     - Specific days of the month (`everyMonthDay`)
     - Specific days of the week (`everyWeekday`)
   - Start date and optional end date

2. **Generation**: Users click "Create recurrences" button (typically every 2 weeks) to:
   - Generate actual task instances for the current and next week
   - Create tasks based on the configured patterns
   - Only create tasks that match the schedule and fall within the visible period

Example: A recurrence with "every Monday" will create one task for each Monday in the next 14 days when "Create recurrences" is clicked.

## Key Features

### Task Management Operations

- **Create**: Add new tasks via modal dialog or quick-add buttons
- **Complete/Uncomplete**: Toggle task completion status
- **Delete**: Mark tasks as deleted (soft delete)
- **Edit**: Modify task properties through edit modal
- **Drag-and-Drop**: Reorder tasks within a day or move between days
- **Time-based Parsing**: Enter tasks with time notation (e.g., "1730 Meeting" creates a task at 17:30)

### User Interface

- **Home/Overview**: Main view showing current and next week with tasks organized by day
- **Recurrences Tab**: Interface for managing recurring task patterns
- **Settings Tab**: User preferences and configuration
- **Real-time Sync**: Changes are synchronized across browser tabs via WebSocket

### Multi-Client Support

The system supports multiple client types:
- **Web Client**: Primary React-based interface
- **Mobile Client**: iOS application
- **Telegram Bot**: Task management via Telegram commands
- **MCP (Model Context Protocol) Client**: AI assistant integration

## Domain Model Summary

### Entities

- **TaskEntity**: Represents a single task with all its properties
- **PlannedRecurrenceEntity**: Defines a recurring task pattern
- **RecurrenceEntity**: Tracks which task instances have been created from a recurrence

### Enums

- **TaskType**: Simple, Additional, Routine, Weekly
- **RecurrenceWeekday**: Monday through Sunday (flags enum for combining days)

## Technical Architecture

- **Frontend**: React + TypeScript with Redux for state management
- **Backend**: ASP.NET Core with C#
- **Databases**:
  - PostgreSQL for authentication
  - MongoDB for task and recurrence storage
- **Communication**: REST APIs + WebSocket for real-time updates

## Development Guidelines

When working with tasks:
1. Respect the 14-day visible period constraint
2. Preserve task type indicators when displaying tasks
3. Handle "No date" tasks as a separate category
4. Consider the drag-and-drop reordering logic when modifying task lists
5. Remember that recurrences are templates, not tasks themselves

When working with recurrences:
1. Recurrences are scheduled patterns, not actual tasks
2. Task generation is manual (via "Create recurrences" button)
3. The system prevents duplicate task creation for the same recurrence and date
4. Generated tasks are independent - editing them doesn't affect the recurrence template
