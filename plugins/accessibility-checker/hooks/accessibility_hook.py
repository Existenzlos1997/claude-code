#!/usr/bin/env python3
"""
Accessibility Checker Hook for Claude Code
This hook checks for accessibility issues in code edits and provides educational feedback.
"""

import json
import os
import random
import re
import sys
from datetime import datetime

# Debug log file
DEBUG_LOG_FILE = "/tmp/accessibility-warnings-log.txt"


def debug_log(message):
    """Append debug message to log file with timestamp."""
    try:
        timestamp = datetime.now().strftime("%Y-%m-%d %H:%M:%S.%f")[:-3]
        with open(DEBUG_LOG_FILE, "a") as f:
            f.write(f"[{timestamp}] {message}\n")
    except Exception:
        pass  # Silently ignore logging errors


# Accessibility patterns configuration
ACCESSIBILITY_PATTERNS = [
    {
        "ruleName": "interactive_div_span",
        "patterns": [
            r"<div[^>]*\sonClick",
            r"<span[^>]*\sonClick",
        ],
        "reminder": """⚠️ Accessibility Warning: Interactive div/span elements are not keyboard accessible

Problems:
1. Not keyboard accessible (cannot be focused with Tab key)
2. Not announced to screen readers as interactive
3. No semantic meaning

Fix: Use a semantic <button> element instead:
  <button onClick={handleClick}>Click me</button>

If you must use a div/span, add:
- role="button"
- tabIndex={0}
- onKeyDown handler for Enter/Space keys

WCAG Criterion: 2.1.1 Keyboard (Level A)
Impact: Keyboard users cannot access this functionality""",
    },
    {
        "ruleName": "missing_alt_text",
        "patterns": [
            r"<img\s+(?![^>]*alt\s*=)",
            r"<img\s+(?![^>]*alt\s*=)[^>]*>",
        ],
        "reminder": """⚠️ Accessibility Warning: Image missing alt text

All images must have an alt attribute:
- Descriptive alt text for meaningful images (conveys purpose/content)
- Empty alt="" for decorative images

Examples:
  <img src="logo.png" alt="Company Logo - Acme Corp" />
  <img src="decorative-line.png" alt="" />  {/* decorative */}

WCAG Criterion: 1.1.1 Non-text Content (Level A)
Impact: Screen reader users cannot understand image content or purpose""",
    },
    {
        "ruleName": "input_without_label",
        "patterns": [
            r"<input(?![^>]*id\s*=)(?![^>]*aria-label)",
        ],
        "reminder": """⚠️ Accessibility Warning: Input missing accessible label

Problems:
1. Placeholder is NOT a substitute for a label
2. Screen readers may not announce the field purpose
3. Clicking label doesn't focus the input

Fix: Add a proper label element:
  <label htmlFor="email">Email Address</label>
  <input id="email" type="email" />

Alternative (if visual label not desired):
  <input type="email" aria-label="Email Address" />

WCAG Criterion: 3.3.2 Labels or Instructions (Level A)
Impact: Users don't know what information to enter""",
    },
    {
        "ruleName": "redundant_button_role",
        "patterns": [
            r"<button[^>]*\srole\s*=\s*['\"]button['\"]",
        ],
        "reminder": """⚠️ Accessibility Warning: Redundant ARIA role on button

The <button> element already has the "button" role implicitly.
Adding role="button" is redundant and can confuse screen readers.

Fix: Remove the redundant role attribute:
  <button onClick={handleClick}>Click me</button>

Note: Only add role="button" to non-semantic elements (div, span) when
creating custom button components with full keyboard support.

WCAG Criterion: 4.1.2 Name, Role, Value (Level A)
Impact: Can cause confusion with assistive technology""",
    },
    {
        "ruleName": "missing_button_type",
        "patterns": [
            r"<button(?![^>]*type\s*=)",
        ],
        "reminder": """⚠️ Accessibility Warning: Button missing explicit type attribute

In HTML forms, buttons default to type="submit", which can cause
unexpected form submissions.

Fix: Always specify the button type:
  <button type="button" onClick={handleClick}>Click me</button>
  <button type="submit">Submit Form</button>
  <button type="reset">Reset Form</button>

Best Practice: Be explicit about button purpose
Impact: Prevents accidental form submissions and improves clarity""",
    },
    # Note: onclick_without_keyboard is covered by interactive_div_span pattern
    # No need for a separate pattern that would trigger on semantic elements
    {
        "ruleName": "aria_hidden_focusable",
        "patterns": [
            r'aria-hidden\s*=\s*["\']true["\'][^>]*(?:tabIndex|href|onClick)',
        ],
        "reminder": """⚠️ Accessibility Warning: Focusable element with aria-hidden="true"

Elements with aria-hidden="true" should not be focusable because this
creates confusion - keyboard users can focus elements that screen readers
can't perceive.

Fix: Either:
1. Remove aria-hidden="true", or
2. Add tabIndex={-1} to make element unfocusable

Example:
  <div aria-hidden="true" tabIndex={-1}>Hidden content</div>

WCAG Criterion: 4.1.2 Name, Role, Value (Level A)
Impact: Creates confusing experience for screen reader users""",
    },
    {
        "ruleName": "missing_dialog_role",
        "patterns": [
            r'className\s*=\s*["\'][^"\']*modal[^"\']*["\'](?![^>]*role\s*=)',
        ],
        "reminder": """⚠️ Accessibility Warning: Modal/dialog missing role attribute

Modal dialogs must have proper ARIA attributes for screen readers.

Fix: Add dialog role and labels:
  <div 
    className="modal"
    role="dialog"
    aria-modal="true"
    aria-labelledby="dialog-title"
  >
    <h2 id="dialog-title">Modal Title</h2>
    ...
  </div>

Also ensure:
- Focus moves to modal when opened
- Focus is trapped within modal
- Focus restores when closed
- ESC key closes modal

WCAG Criterion: 4.1.2 Name, Role, Value (Level A)
Impact: Screen reader users don't know they've entered a dialog""",
    },
    {
        "ruleName": "heading_skip_level",
        "patterns": [
            r"<h1[^>]*>.*?</h1>.*?<h3",
            r"<h2[^>]*>.*?</h2>.*?<h4",
            r"<h3[^>]*>.*?</h3>.*?<h5",
        ],
        "reminder": """⚠️ Accessibility Warning: Heading hierarchy might skip levels

Heading levels should not skip (e.g., h1 → h3 skipping h2).
Screen readers use headings for navigation.

Correct hierarchy:
  <h1>Page Title</h1>
    <h2>Section</h2>
      <h3>Subsection</h3>
      <h3>Another Subsection</h3>
    <h2>Another Section</h2>

Fix: Ensure logical heading order
Note: This is a pattern check - verify the actual heading flow

WCAG Criterion: 1.3.1 Info and Relationships (Level A)
Impact: Screen reader users rely on headings for navigation""",
    },
    {
        "ruleName": "positive_tabindex",
        "patterns": [
            r'tabIndex\s*=\s*["{]([1-9]\d*)',
        ],
        "reminder": """⚠️ Accessibility Warning: Positive tabIndex value detected

Positive tabIndex values (1, 2, 3, etc.) are an anti-pattern because:
1. They override natural tab order
2. Make code hard to maintain
3. Confuse keyboard users

Use these values only:
- tabIndex={0} - Include in natural tab order
- tabIndex={-1} - Remove from tab order (still focusable programmatically)

Let DOM order determine tab order by default.

WCAG Criterion: 2.4.3 Focus Order (Level A)
Impact: Creates unpredictable, confusing navigation for keyboard users""",
    },
]


def get_state_file(session_id):
    """Get session-specific state file path."""
    return os.path.expanduser(f"~/.claude/a11y_warnings_state_{session_id}.json")


def cleanup_old_state_files():
    """Remove state files older than 30 days."""
    try:
        state_dir = os.path.expanduser("~/.claude")
        if not os.path.exists(state_dir):
            return

        current_time = datetime.now().timestamp()
        thirty_days_ago = current_time - (30 * 24 * 60 * 60)

        for filename in os.listdir(state_dir):
            if filename.startswith("a11y_warnings_state_") and filename.endswith(
                ".json"
            ):
                file_path = os.path.join(state_dir, filename)
                try:
                    file_mtime = os.path.getmtime(file_path)
                    if file_mtime < thirty_days_ago:
                        os.remove(file_path)
                except (OSError, IOError):
                    pass
    except Exception:
        pass


def load_state(session_id):
    """Load the state of shown warnings from file."""
    state_file = get_state_file(session_id)
    if os.path.exists(state_file):
        try:
            with open(state_file, "r") as f:
                return set(json.load(f))
        except (json.JSONDecodeError, IOError):
            return set()
    return set()


def save_state(session_id, shown_warnings):
    """Save the state of shown warnings to file."""
    state_file = get_state_file(session_id)
    try:
        os.makedirs(os.path.dirname(state_file), exist_ok=True)
        with open(state_file, "w") as f:
            json.dump(list(shown_warnings), f)
    except IOError as e:
        debug_log(f"Failed to save state file: {e}")


def check_patterns(file_path, content):
    """Check if content matches any accessibility patterns."""
    # Only check relevant file types
    if not any(
        file_path.endswith(ext)
        for ext in [".jsx", ".tsx", ".vue", ".html", ".svelte", ".js", ".ts"]
    ):
        return None, None

    for pattern in ACCESSIBILITY_PATTERNS:
        for regex in pattern["patterns"]:
            if re.search(regex, content, re.MULTILINE | re.DOTALL):
                return pattern["ruleName"], pattern["reminder"]

    return None, None


def extract_content_from_input(tool_name, tool_input):
    """Extract content to check from tool input based on tool type."""
    if tool_name == "Write":
        return tool_input.get("content", "")
    elif tool_name == "Edit":
        return tool_input.get("new_string", "")
    elif tool_name == "MultiEdit":
        edits = tool_input.get("edits", [])
        if edits:
            return " ".join(edit.get("new_string", "") for edit in edits)
        return ""
    return ""


def main():
    """Main hook function."""
    # Check if accessibility checker is enabled
    a11y_checker_enabled = os.environ.get("ENABLE_A11Y_CHECKER", "1")

    if a11y_checker_enabled == "0":
        sys.exit(0)

    # Periodically clean up old state files
    if random.random() < 0.1:
        cleanup_old_state_files()

    # Read input from stdin
    try:
        raw_input = sys.stdin.read()
        input_data = json.loads(raw_input)
    except json.JSONDecodeError as e:
        debug_log(f"JSON decode error: {e}")
        sys.exit(0)

    # Extract session ID and tool information
    session_id = input_data.get("session_id", "default")
    tool_name = input_data.get("tool_name", "")
    tool_input = input_data.get("tool_input", {})

    # Check if this is a relevant tool
    if tool_name not in ["Edit", "Write", "MultiEdit"]:
        sys.exit(0)

    # Extract file path from tool_input
    file_path = tool_input.get("file_path", "")
    if not file_path:
        sys.exit(0)

    # Extract content to check
    content = extract_content_from_input(tool_name, tool_input)

    # Check for accessibility patterns
    rule_name, reminder = check_patterns(file_path, content)

    if rule_name and reminder:
        # Create unique warning key
        warning_key = f"{file_path}-{rule_name}"

        # Load existing warnings for this session
        shown_warnings = load_state(session_id)

        # Check if we've already shown this warning in this session
        if warning_key not in shown_warnings:
            # Add to shown warnings and save
            shown_warnings.add(warning_key)
            save_state(session_id, shown_warnings)

            # Output the warning to stderr and block execution
            print(reminder, file=sys.stderr)
            sys.exit(2)  # Block tool execution

    # Allow tool to proceed
    sys.exit(0)


if __name__ == "__main__":
    main()
