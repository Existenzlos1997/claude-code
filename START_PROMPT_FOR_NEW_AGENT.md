# Creating a New Agent for Claude Code

This guide will help you create a new specialized agent for Claude Code. Agents are autonomous assistants that can be triggered automatically based on user requests or explicitly invoked to perform specific tasks.

## Table of Contents

1. [Understanding Agents](#understanding-agents)
2. [Agent Structure](#agent-structure)
3. [Step-by-Step Creation Guide](#step-by-step-creation-guide)
4. [Configuration Reference](#configuration-reference)
5. [Best Practices](#best-practices)
6. [Examples](#examples)
7. [Plugin Organization](#plugin-organization)

## Understanding Agents

Agents in Claude Code are specialized assistants that:
- **Focus on specific tasks**: Each agent has a clear, narrow responsibility (e.g., code review, test analysis, architecture design)
- **Trigger automatically**: Claude can detect when to use an agent based on user requests
- **Run in separate contexts**: Agents execute in isolated environments, keeping the main conversation clean
- **Return focused results**: Agents provide concise, actionable output without cluttering the main context

### When to Create an Agent

Create an agent when you have a task that:
- Is well-defined and repeatable
- Requires specialized expertise or analysis
- Benefits from isolation (to avoid context pollution)
- Should trigger automatically based on user intent
- Needs to run in parallel with other tasks

### Agent vs. Command

- **Agents**: Autonomous assistants that can be triggered automatically or manually. They analyze, review, or generate insights.
- **Commands**: Guided workflows that follow a specific multi-step process. They coordinate multiple actions to accomplish a goal.

## Agent Structure

Agents are defined using Markdown files with YAML frontmatter. Here's the basic structure:

```markdown
---
name: agent-name
description: Brief description of what the agent does
tools: Tool1, Tool2, Tool3
model: sonnet
color: blue
---

# Agent Instructions

Your detailed instructions for how the agent should behave...

## Core Responsibilities

What the agent should focus on...

## Output Guidance

How the agent should format its response...
```

### Required Fields

- **name**: Unique identifier for the agent (kebab-case)
- **description**: Clear, concise description that helps Claude know when to trigger this agent
- **tools**: Comma-separated list of tools the agent can use
- **model**: Which AI model to use (`sonnet`, `opus`, or `inherit`)
- **color**: Visual identifier in the UI (`red`, `green`, `blue`, `yellow`, `cyan`, `pink`)

## Step-by-Step Creation Guide

### Step 1: Define Your Agent's Purpose

Before writing any code, clearly answer:
1. **What specific problem does this agent solve?**
2. **When should it be triggered?**
3. **What information does it need?**
4. **What output should it produce?**

Example:
- **Purpose**: Review test coverage for completeness
- **Trigger**: When user asks about test coverage or creates a PR
- **Input**: Changed files in PR, existing test files
- **Output**: List of critical test gaps with severity ratings

### Step 2: Choose the Right Tools

Available tools for agents:
- **Glob**: Fast file pattern matching
- **Grep**: Search file contents using regex
- **LS**: List directory contents
- **Read**: Read file contents
- **NotebookRead**: Read and analyze notebook files
- **WebFetch**: Fetch content from URLs
- **TodoWrite**: Create and update todo lists
- **WebSearch**: Search the web for information
- **KillShell**: Terminate shell processes
- **BashOutput**: Execute bash commands and get output

Choose only the tools your agent actually needs. Fewer tools = clearer purpose.

### Step 3: Select the Appropriate Model

- **sonnet**: Default choice. Fast, capable, cost-effective. Use for most agents.
- **opus**: Most powerful. Use only for complex reasoning or critical analysis.
- **inherit**: Use the same model as the parent conversation.

### Step 4: Write Clear Instructions

Your agent's markdown content should include:

1. **Role Definition**: Start with "You are an expert..." to set the context
2. **Core Responsibilities**: List the specific tasks the agent should perform
3. **Analysis Approach**: Step-by-step process the agent should follow
4. **Output Guidance**: Clear instructions on how to format results
5. **Examples**: Show what good output looks like (optional but helpful)

### Step 5: Create the Agent File

Create a new markdown file in the appropriate location:
- For plugin agents: `plugins/[plugin-name]/agents/[agent-name].md`
- For project agents: `.claude/agents/[agent-name].md`
- For global agents: `~/.claude/agents/[agent-name].md`

### Step 6: Test Your Agent

1. Ask Claude to use your agent explicitly: "Use the [agent-name] agent to..."
2. Try triggering it automatically with relevant questions
3. Review the output for clarity and usefulness
4. Iterate on the instructions based on results

## Configuration Reference

### YAML Frontmatter Fields

```yaml
---
name: my-agent              # Required: kebab-case identifier
description: What it does   # Required: Helps with auto-triggering
tools: Read, Grep, Glob     # Required: Comma-separated list
model: sonnet               # Required: sonnet, opus, or inherit
color: blue                 # Required: UI color identifier
---
```

### Available Colors

- `red`: For critical/review agents
- `green`: For creation/architecture agents  
- `yellow`: For exploration/analysis agents
- `blue`: For information/research agents
- `cyan`: For testing/validation agents
- `pink`: For documentation/writing agents

### Model Selection Guide

| Model | Use For | Speed | Cost | Capability |
|-------|---------|-------|------|------------|
| sonnet | Most agents | Fast | Low | High |
| opus | Complex analysis | Slow | High | Highest |
| inherit | Context-dependent | Varies | Varies | Varies |

## Best Practices

### 1. Clear, Narrow Focus

❌ **Bad**: "Review code for any issues"
✅ **Good**: "Review error handling for silent failures in catch blocks"

### 2. Actionable Output

Always provide:
- Specific file and line references
- Clear explanation of the issue
- Concrete suggestions for improvement
- Priority/severity ratings

### 3. Confidence Scoring

For review agents, include confidence ratings:
```markdown
Rate each finding on a scale of 0-100:
- 0-50: Uncertain or low priority
- 51-79: Moderate confidence
- 80-100: High confidence, definitely report
```

### 4. Structured Instructions

Use clear sections:
```markdown
## Core Responsibilities
What to do...

## Analysis Process
How to do it...

## Output Format
How to present results...
```

### 5. Avoid Overlap

Check existing agents before creating a new one. If similar functionality exists, consider:
- Extending an existing agent
- Creating a more specialized variant
- Combining related tasks

### 6. Test Trigger Phrases

Document example phrases that should trigger your agent:
```markdown
## Trigger Examples
- "Check test coverage"
- "Are there any test gaps?"
- "Review the tests in this PR"
```

## Examples

### Example 1: Simple Review Agent

```markdown
---
name: error-handler-reviewer
description: Reviews error handling for silent failures and inadequate error logging
tools: Grep, Read
model: sonnet
color: red
---

You are an expert in error handling best practices across multiple programming languages.

## Core Responsibilities

Review code for:
1. Silent failures in catch blocks
2. Missing error logging
3. Inappropriate fallback behavior
4. Swallowed exceptions

## Analysis Process

1. Find all try/catch blocks in changed files
2. Analyze each catch block for:
   - Presence of error logging
   - Appropriate error propagation
   - User-facing error messages
3. Rate severity of each issue (1-10)

## Output Format

For each issue found:
- File and line number
- Severity (1-10)
- Description of the problem
- Suggested fix

Only report issues with severity ≥ 7.
```

### Example 2: Analysis Agent

```markdown
---
name: performance-analyzer
description: Analyzes code for performance bottlenecks and optimization opportunities
tools: Read, Grep, Glob
model: sonnet
color: yellow
---

You are a performance optimization expert.

## Core Responsibilities

Identify performance issues:
1. Inefficient algorithms (O(n²) or worse)
2. Unnecessary loops or iterations
3. Missing caching opportunities
4. Database query inefficiencies

## Analysis Approach

1. Read changed files
2. Identify hot paths (frequently executed code)
3. Analyze algorithmic complexity
4. Check for common anti-patterns

## Output Guidance

Provide:
- Current complexity vs. optimal complexity
- Specific optimization suggestions
- Expected performance impact
- Code examples of improvements
```

### Example 3: Architecture Agent

```markdown
---
name: api-designer
description: Designs RESTful API architectures following best practices and existing patterns
tools: Read, Grep, Glob, WebSearch
model: opus
color: green
---

You are a senior API architect.

## Core Process

1. **Pattern Analysis**: Study existing API endpoints to understand conventions
2. **Design**: Create comprehensive API specification
3. **Documentation**: Provide OpenAPI/Swagger definitions

## Design Principles

- RESTful conventions
- Consistent naming and structure
- Proper HTTP status codes
- Comprehensive error responses
- Pagination for collections
- Versioning strategy

## Output Deliverables

1. Complete endpoint specifications
2. Request/response schemas
3. Error handling design
4. Example requests and responses
```

## Plugin Organization

### Creating a Plugin

Plugins bundle related agents and commands. Structure:

```
plugins/
  my-plugin/
    .claude-plugin/
      plugin.json         # Plugin metadata
    agents/
      agent1.md          # Agent definitions
      agent2.md
    commands/
      command1.md        # Command definitions
    README.md            # Documentation
```

### plugin.json Format

```json
{
  "name": "my-plugin",
  "version": "1.0.0",
  "description": "Description of what this plugin does",
  "author": {
    "name": "Your Name",
    "email": "your.email@example.com"
  }
}
```

### Plugin README Template

```markdown
# Plugin Name

Brief description of the plugin.

## Overview

What problem does this plugin solve?

## Agents

### agent-name
**Focus**: What it does

**When to use**: When to trigger it

**Triggers**:
- "Example trigger phrase 1"
- "Example trigger phrase 2"

## Usage

How to use the plugin effectively.

## Installation

How to install (if applicable).
```

## Common Patterns

### Pattern 1: Review Agent

```markdown
---
name: [type]-reviewer
description: Reviews [aspect] for [specific issues]
tools: Read, Grep, Glob
model: sonnet
color: red
---

You are an expert in [domain].

## Review Scope
By default, review unstaged changes from `git diff`.

## Core Responsibilities
List what to check for...

## Confidence Scoring
Rate issues 0-100, only report ≥ 80.

## Output Format
Clear, actionable feedback with file:line references.
```

### Pattern 2: Exploration Agent

```markdown
---
name: [domain]-explorer
description: Analyzes [feature/area] by tracing execution and mapping architecture
tools: Read, Grep, Glob
model: sonnet
color: yellow
---

You are an expert code analyst.

## Core Mission
Provide complete understanding of how [feature] works.

## Analysis Approach
1. Find entry points
2. Trace execution flow
3. Map architecture
4. Document findings

## Output Guidance
Include file:line references, execution flow, key insights.
```

### Pattern 3: Design Agent

```markdown
---
name: [domain]-architect
description: Designs [type] architectures following project patterns and best practices
tools: Read, Grep, Glob
model: opus
color: green
---

You are a senior [domain] architect.

## Core Process
1. Analyze existing patterns
2. Design solution
3. Provide implementation blueprint

## Output Deliverables
- Architecture decision with rationale
- Component designs
- Implementation steps
- File paths and changes
```

## Tips for Success

1. **Start Simple**: Create a basic agent first, then iterate based on usage
2. **Test Thoroughly**: Use the agent multiple times before considering it complete
3. **Clear Descriptions**: The description field is crucial for auto-triggering
4. **Minimal Tools**: Only include tools the agent actually needs
5. **Document Triggers**: Help users know when to use your agent
6. **Iterate Based on Feedback**: Improve instructions based on actual results
7. **Follow Existing Patterns**: Study successful agents before creating new ones

## Resources

- View existing agents in `plugins/*/agents/` for inspiration
- Check `plugins/pr-review-toolkit/README.md` for comprehensive examples
- Read `plugins/feature-dev/commands/feature-dev.md` for workflow patterns

## Next Steps

1. Identify a specific need in your workflow
2. Design your agent following this guide
3. Create the agent file with clear instructions
4. Test with explicit and automatic triggers
5. Iterate based on results
6. Document usage patterns
7. Share with your team or contribute to the repository

Happy agent building! 🚀
