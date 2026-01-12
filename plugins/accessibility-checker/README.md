# Accessibility Checker Plugin

A comprehensive accessibility analysis tool that helps developers create inclusive, WCAG-compliant applications by proactively detecting accessibility barriers in code.

## Why This Matters

Over 1 billion people worldwide live with disabilities. Digital accessibility isn't just a legal requirement—it's a fundamental human right. This plugin helps developers:

- **Prevent accessibility barriers** before they reach production
- **Learn accessibility best practices** through contextual education
- **Ensure WCAG compliance** across multiple frameworks
- **Create inclusive experiences** for all users

## What It Does

The Accessibility Checker plugin provides:

1. **Proactive Code Analysis**: Detects accessibility issues as you write code
2. **Multi-Framework Support**: Works with HTML, React, Vue, Angular, and more
3. **Educational Feedback**: Explains why issues matter and how to fix them
4. **WCAG Compliance**: Checks against WCAG 2.1 Level A and AA standards
5. **Automated Fixes**: Suggests specific, actionable solutions

## Features

### Core Capabilities

- **Semantic HTML Analysis**: Ensures proper use of semantic elements
- **ARIA Attribute Validation**: Checks for correct ARIA usage
- **Keyboard Navigation**: Detects keyboard accessibility issues
- **Color Contrast**: Identifies potential contrast problems
- **Alt Text Detection**: Ensures images have descriptive alternatives
- **Form Accessibility**: Validates form labels and error handling
- **Focus Management**: Checks focus indicators and tab order
- **Screen Reader Compatibility**: Identifies screen reader barriers

### Supported Frameworks

- HTML/CSS/JavaScript
- React (JSX/TSX)
- Vue.js
- Angular
- Svelte
- And more...

## Installation

Install from your personal marketplace:

```bash
/plugins
# Find "accessibility-checker"
# Install
```

## Usage

### Automatic Hook

The plugin runs automatically when you edit files, providing real-time feedback:

```javascript
// You type:
<div onClick={handleClick}>Click me</div>

// Plugin warns:
⚠️ Accessibility Warning: Interactive div elements are not keyboard accessible
Use a <button> element instead, or add role="button" and keyboard event handlers
```

### Agent Commands

Use the accessibility-checker agent for comprehensive reviews:

```bash
# Check current changes for accessibility issues
/accessibility-checker:check-a11y

# Full accessibility audit
/accessibility-checker:audit-a11y all

# Check specific aspects
/accessibility-checker:check-a11y semantic-html
/accessibility-checker:check-a11y aria
/accessibility-checker:check-a11y keyboard
```

### Manual Agent Call

For targeted accessibility analysis:

```
"Check this component for accessibility issues"
"Review the form for WCAG compliance"
"Analyze keyboard navigation in this file"
```

## Agents

### accessibility-reviewer

**Focus**: Comprehensive WCAG 2.1 compliance analysis

**Analyzes:**
- Semantic HTML structure
- ARIA attributes and roles
- Keyboard navigation patterns
- Focus management
- Color contrast issues (code-level)
- Form accessibility
- Image alt text
- Interactive element accessibility
- Screen reader compatibility

**When to use:**
- Before committing changes to UI components
- After implementing new features
- During PR review
- When adding interactive elements

**Triggers:**
```
"Check accessibility of this component"
"Review WCAG compliance"
"Analyze keyboard navigation"
"Check if this is accessible to screen readers"
```

## Hook System

The plugin includes a PreToolUse hook that checks code edits for common accessibility issues:

### Checked Patterns

1. **Interactive Non-Semantic Elements**
   - `<div onClick>`, `<span onClick>` without proper ARIA
   - Missing keyboard event handlers

2. **Missing Alt Text**
   - `<img>` without alt attribute
   - Decorative images without alt=""

3. **Form Labels**
   - Inputs without associated labels
   - Missing form error announcements

4. **ARIA Misuse**
   - Invalid ARIA attributes
   - Redundant ARIA on semantic elements
   - Missing required ARIA attributes

5. **Color Contrast**
   - Hard-coded colors without sufficient contrast
   - Color-only information conveyance

6. **Focus Management**
   - Missing focus indicators
   - Improper focus trapping
   - Negative tabindex on interactive elements

7. **Heading Structure**
   - Skipped heading levels
   - Missing page structure

## Examples

### Example 1: Interactive Element

**Before:**
```jsx
<div className="button" onClick={() => setOpen(true)}>
  Open Menu
</div>
```

**Issue Detected:**
```
⚠️ Accessibility Warning: Interactive div elements are not accessible

Problems:
1. Not keyboard accessible (no keyboard event handlers)
2. Not announced to screen readers (no role)
3. Not focusable (no tabindex)

Fix: Use a semantic button element
```

**After:**
```jsx
<button className="button" onClick={() => setOpen(true)}>
  Open Menu
</button>
```

### Example 2: Image Alt Text

**Before:**
```jsx
<img src="logo.png" />
```

**Issue Detected:**
```
⚠️ Accessibility Warning: Image missing alt text

All images must have an alt attribute:
- Descriptive alt text for meaningful images
- Empty alt="" for decorative images

Fix: Add descriptive alt text
```

**After:**
```jsx
<img src="logo.png" alt="Company Logo - Acme Corp" />
```

### Example 3: Form Labels

**Before:**
```jsx
<input type="email" placeholder="Enter email" />
```

**Issue Detected:**
```
⚠️ Accessibility Warning: Input missing accessible label

Problems:
1. Placeholder is not a substitute for a label
2. Screen readers may not announce the purpose
3. Placeholder disappears when typing

Fix: Add a proper label element
```

**After:**
```jsx
<label htmlFor="email">Email Address</label>
<input id="email" type="email" placeholder="Enter email" />
```

### Example 4: ARIA Usage

**Before:**
```jsx
<button role="button">Click me</button>
```

**Issue Detected:**
```
⚠️ Accessibility Warning: Redundant ARIA role

The <button> element already has the "button" role implicitly.
Adding role="button" is redundant and can confuse screen readers.

Fix: Remove the redundant role attribute
```

**After:**
```jsx
<button>Click me</button>
```

## Severity Levels

The plugin categorizes issues by severity:

### Critical (Must Fix)
- Blocking keyboard navigation
- Missing form labels
- Improper ARIA breaking screen readers
- Interactive elements not accessible

### Important (Should Fix)
- Missing alt text
- Insufficient semantic structure
- Missing focus indicators
- Color contrast issues

### Advisory (Consider Fixing)
- Redundant ARIA
- Best practice improvements
- Enhanced screen reader support

## WCAG Guidelines Covered

This plugin helps ensure compliance with:

- **WCAG 2.1 Level A** (minimum requirements)
- **WCAG 2.1 Level AA** (recommended standard)

### Key Success Criteria

- 1.1.1 Non-text Content (A)
- 1.3.1 Info and Relationships (A)
- 1.4.3 Contrast (Minimum) (AA)
- 2.1.1 Keyboard (A)
- 2.1.2 No Keyboard Trap (A)
- 2.4.1 Bypass Blocks (A)
- 2.4.3 Focus Order (A)
- 2.4.6 Headings and Labels (AA)
- 2.4.7 Focus Visible (AA)
- 3.2.2 On Input (A)
- 3.3.1 Error Identification (A)
- 3.3.2 Labels or Instructions (A)
- 4.1.2 Name, Role, Value (A)

## Best Practices

### When to Use

**During Development:**
- Real-time feedback via hooks as you code
- Catch issues immediately

**Before Committing:**
- Run `/accessibility-checker:check-a11y`
- Review and fix critical issues

**Before Creating PR:**
- Run `/accessibility-checker:audit-a11y all`
- Ensure comprehensive compliance

**During PR Review:**
- Use agent for specific component analysis
- Verify accessibility requirements met

### Integration Workflow

```
1. Write/modify UI code
2. Hook provides real-time feedback
3. Fix critical accessibility issues
4. Run check-a11y command
5. Address all issues found
6. Commit changes
7. Before PR: run full audit
8. Create PR with accessibility confidence
```

## Educational Resources

The plugin provides contextual education:

- **Why it matters**: Explains impact on users with disabilities
- **How to fix**: Specific, actionable solutions
- **Best practices**: Industry standards and guidelines
- **WCAG references**: Links to relevant success criteria

## Customization

### Disable Specific Checks

Set environment variables:

```bash
export DISABLE_A11Y_INTERACTIVE_DIV=1
export DISABLE_A11Y_ALT_TEXT=1
```

### Configure Severity

Customize what blocks code execution in `.claude/accessibility-config.json`:

```json
{
  "blockOnSeverity": "critical",
  "enabledChecks": [
    "semantic-html",
    "aria",
    "keyboard",
    "alt-text",
    "forms"
  ]
}
```

## Contributing

Found an accessibility pattern we should check? Submit issues or PRs!

## Resources

- [WCAG 2.1 Guidelines](https://www.w3.org/WAI/WCAG21/quickref/)
- [MDN Accessibility](https://developer.mozilla.org/en-US/docs/Web/Accessibility)
- [A11y Project](https://www.a11yproject.com/)
- [WebAIM](https://webaim.org/)

## Impact

By using this plugin, you're helping create a more inclusive digital world. Every accessibility issue you prevent makes the web more usable for:

- People using screen readers
- People with motor disabilities using keyboard navigation
- People with visual impairments needing high contrast
- People with cognitive disabilities needing clear structure
- Everyone benefiting from better UX

## License

MIT

## Author

Claude Code Community

---

**Making the web accessible, one commit at a time. 🌐♿**
