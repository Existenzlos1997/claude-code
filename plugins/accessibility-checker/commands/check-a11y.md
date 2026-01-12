---
description: "Quick accessibility check of recent changes"
argument-hint: "[aspect]"
allowed-tools: ["Bash", "Glob", "Grep", "Read", "Task"]
---

# Accessibility Check

Run a quick accessibility check on recent code changes, focusing on common WCAG compliance issues.

**Check Aspect (optional):** "$ARGUMENTS"

## Check Workflow:

1. **Determine Scope**
   - Check git status to identify changed files
   - Parse arguments for specific aspect to check
   - Default: Check all accessibility aspects

2. **Available Check Aspects:**

   - **semantic-html** - Check for proper semantic element usage
   - **aria** - Validate ARIA attributes and roles
   - **keyboard** - Check keyboard navigation patterns
   - **alt-text** - Verify image alt text
   - **forms** - Check form labels and accessibility
   - **focus** - Validate focus management
   - **all** - Check all aspects (default)

3. **Identify Changed Files**
   - Run `git diff --name-only` for modified files
   - Filter for relevant file types:
     - `.jsx`, `.tsx` - React components
     - `.vue` - Vue components
     - `.html` - HTML files
     - `.ts`, `.js` - TypeScript/JavaScript with UI code
     - `.svelte` - Svelte components

4. **Run Accessibility Checks**

   Based on aspect requested or default to all:
   
   **semantic-html**:
   - Check for divs/spans used instead of buttons
   - Verify heading hierarchy
   - Ensure landmark regions
   
   **aria**:
   - Check ARIA role usage
   - Verify required ARIA attributes
   - Detect redundant ARIA on semantic elements
   
   **keyboard**:
   - Check onClick without onKeyDown/onKeyPress
   - Verify tabindex usage
   - Check for keyboard traps
   
   **alt-text**:
   - Find images without alt attributes
   - Check for meaningful alt text
   - Verify decorative images have alt=""
   
   **forms**:
   - Check inputs have associated labels
   - Verify error message accessibility
   - Check required field indication
   
   **focus**:
   - Check for focus indicator styles
   - Verify focus order is logical
   - Check focus management in dynamic components

5. **Launch Agent if Needed**

   For complex analysis or multiple issues, launch the accessibility-reviewer agent:
   
   ```bash
   # Use Task tool to launch agent
   /task accessibility-reviewer "Review files: <file list>"
   ```

6. **Provide Results**

   Report findings in this format:
   
   ```markdown
   # Accessibility Check Results
   
   Checked: X files for <aspect>
   
   ## Critical Issues (Must Fix)
   - [file:line] Issue description
   
   ## Important Issues (Should Fix)
   - [file:line] Issue description
   
   ## Summary
   - ✅ Passed: X checks
   - ⚠️  Warnings: X issues
   - ❌ Errors: X critical issues
   
   ## Next Steps
   1. Fix critical issues first
   2. Run full audit before PR: /accessibility-checker:audit-a11y
   ```

## Usage Examples:

**Quick check (default):**
```
/accessibility-checker:check-a11y
```

**Check specific aspect:**
```
/accessibility-checker:check-a11y semantic-html
# Checks only semantic HTML usage

/accessibility-checker:check-a11y aria
# Checks only ARIA attributes

/accessibility-checker:check-a11y keyboard
# Checks only keyboard accessibility
```

**Check multiple aspects:**
```
/accessibility-checker:check-a11y semantic-html aria keyboard
# Checks semantic HTML, ARIA, and keyboard navigation
```

## Quick Check Patterns

### Pattern 1: Interactive Divs/Spans

Search for:
- `<div onClick` or `<span onClick`
- Without `role="button"` and keyboard handlers

**Fix**: Use `<button>` element instead

### Pattern 2: Missing Alt Text

Search for:
- `<img` without `alt=`
- `<img alt="">` on meaningful images

**Fix**: Add descriptive alt text

### Pattern 3: Form Labels

Search for:
- `<input` without associated `<label>`
- `<input` without `id` when label has `htmlFor`

**Fix**: Add proper label association

### Pattern 4: Redundant ARIA

Search for:
- `<button role="button">`
- `<a role="link">`
- `<input role="textbox">`

**Fix**: Remove redundant roles

### Pattern 5: Missing Keyboard Handlers

Search for:
- `onClick` without `onKeyDown` or `onKeyPress`
- On non-semantic elements (div, span)

**Fix**: Add keyboard event handlers or use semantic elements

## Tips:

- **Run early**: Check accessibility as you code
- **Fix immediately**: Easier to fix issues while context is fresh
- **Use full audit before PR**: This is a quick check, not comprehensive
- **Learn from feedback**: Each issue is an opportunity to improve
- **Test manually**: Automated checks can't catch everything

## Integration:

**Before committing:**
```
1. Write/modify UI code
2. Run: /accessibility-checker:check-a11y
3. Fix any critical issues found
4. Commit
```

**During development:**
```
1. Make UI changes
2. Quick check: /accessibility-checker:check-a11y <specific-aspect>
3. Fix and iterate
```

## Note:

This is a quick check for common issues. For comprehensive WCAG compliance review, use `/accessibility-checker:audit-a11y all` before creating PRs.
