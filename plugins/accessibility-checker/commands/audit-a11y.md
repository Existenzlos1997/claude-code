---
description: "Comprehensive WCAG 2.1 accessibility audit"
argument-hint: "[scope]"
allowed-tools: ["Bash", "Glob", "Grep", "Read", "Task"]
---

# Comprehensive Accessibility Audit

Run a thorough WCAG 2.1 Level AA accessibility audit on your codebase, identifying all accessibility barriers and providing detailed remediation guidance.

**Audit Scope (optional):** "$ARGUMENTS"

## Audit Workflow:

1. **Determine Audit Scope**

   Parse arguments or default to changed files:
   
   - **all** - Audit entire codebase
   - **changes** - Audit only git diff (default)
   - **components** - Audit component directories
   - **<directory>** - Audit specific directory
   - **<file>** - Audit specific file

2. **Identify Files to Audit**

   Find all relevant UI files in scope:
   ```bash
   # For React/TypeScript projects
   find <scope> -name "*.tsx" -o -name "*.jsx"
   
   # For Vue projects
   find <scope> -name "*.vue"
   
   # For HTML
   find <scope> -name "*.html"
   
   # For Svelte
   find <scope> -name "*.svelte"
   ```

3. **Prepare Audit Context**

   Gather information for comprehensive analysis:
   - List of all files to audit
   - Project framework (React, Vue, Angular, etc.)
   - Existing accessibility patterns in codebase
   - CLAUDE.md guidelines if present

4. **Launch Comprehensive Agent**

   Use the accessibility-reviewer agent for thorough analysis:
   ```
   Launch agent with full context:
   - Files to review
   - Framework information
   - Request severity scoring
   - Request WCAG mapping
   ```

5. **Agent Analysis**

   The agent will comprehensively check:

   **A. Semantic HTML** (WCAG 1.3.1, 4.1.2)
   - Proper element usage
   - Heading hierarchy
   - Landmark regions
   - Semantic structure

   **B. ARIA Implementation** (WCAG 4.1.2)
   - Correct roles
   - Required attributes
   - State management
   - Relationships
   - Live regions

   **C. Keyboard Navigation** (WCAG 2.1.1, 2.1.2, 2.4.3)
   - Keyboard accessibility
   - Tab order
   - Focus trapping
   - Skip links
   - No keyboard traps

   **D. Focus Management** (WCAG 2.4.7)
   - Visible focus
   - Focus order
   - Focus restoration
   - Focus styling

   **E. Image Accessibility** (WCAG 1.1.1)
   - Alt text presence
   - Alt text quality
   - Decorative images
   - Complex images
   - SVG accessibility

   **F. Form Accessibility** (WCAG 1.3.1, 3.3.1, 3.3.2)
   - Label association
   - Error messages
   - Required fields
   - Fieldset/legend
   - Validation feedback

   **G. Color & Contrast** (WCAG 1.4.1, 1.4.3)
   - Color-only information
   - Contrast issues (code level)
   - Visual indicators

   **H. Screen Reader Compatibility** (WCAG 1.1.1, 2.4.4, 4.1.2)
   - Link text
   - Hidden content
   - Dynamic updates
   - Reading order
   - Icon alternatives

   **I. Interactive Elements** (WCAG 4.1.2)
   - Button vs. link usage
   - Button types
   - Disabled states
   - Loading states
   - Toggle buttons

   **J. Dynamic Content** (WCAG 4.1.3)
   - Live regions
   - Loading states
   - Error announcements
   - Success messages
   - Client-side routing

6. **Aggregate Audit Results**

   Compile comprehensive report:

   ```markdown
   # WCAG 2.1 Accessibility Audit Report
   
   **Audit Date**: <timestamp>
   **Scope**: <files/directories audited>
   **Files Analyzed**: X files
   **Framework**: <detected framework>
   
   ---
   
   ## Executive Summary
   
   - Total Issues: X
   - Critical (9-10): X issues
   - Important (7-8): X issues
   - WCAG Level: <A/AA/AAA or failing>
   
   ---
   
   ## Critical Issues (9-10)
   
   Must fix before production deployment.
   
   ### Issue 1: [Title] [Severity: X, WCAG X.X.X]
   **File**: path/to/file.tsx:line
   **Impact**: How this affects users
   **Current Code**: ...
   **Fix**: ...
   **WCAG Criterion**: X.X.X Description
   
   ---
   
   ## Important Issues (7-8)
   
   Should fix for WCAG AA compliance.
   
   ### Issue X: [Title] [Severity: X, WCAG X.X.X]
   ...
   
   ---
   
   ## Accessibility Strengths
   
   ✅ Positive patterns observed
   ✅ Good practices in place
   
   ---
   
   ## WCAG 2.1 Compliance Matrix
   
   | Criterion | Level | Status | Issues |
   |-----------|-------|--------|--------|
   | 1.1.1 Non-text Content | A | ❌ | 3 |
   | 2.1.1 Keyboard | A | ✅ | 0 |
   | 2.4.7 Focus Visible | AA | ⚠️ | 1 |
   ...
   
   ---
   
   ## Recommended Action Plan
   
   **Phase 1: Critical Fixes** (Must complete)
   1. [Issue description] - File(s)
   2. ...
   
   **Phase 2: Important Fixes** (WCAG AA)
   1. [Issue description] - File(s)
   2. ...
   
   **Phase 3: Enhancements** (Best practices)
   1. [Suggestion]
   2. ...
   
   ---
   
   ## Testing Recommendations
   
   After fixes, test with:
   - ✅ Keyboard-only navigation
   - ✅ NVDA screen reader (Windows)
   - ✅ JAWS screen reader (Windows)
   - ✅ VoiceOver screen reader (macOS/iOS)
   - ✅ TalkBack screen reader (Android)
   - ✅ axe DevTools browser extension
   - ✅ WAVE accessibility checker
   - ✅ Color contrast analyzer
   - ✅ Zoom to 200%
   
   ---
   
   ## Resources
   
   - [WCAG 2.1 Quick Reference](https://www.w3.org/WAI/WCAG21/quickref/)
   - [MDN Accessibility](https://developer.mozilla.org/en-US/docs/Web/Accessibility)
   - Framework-specific guides
   
   ---
   
   ## Next Steps
   
   1. Review and prioritize critical issues
   2. Assign fixes to team members
   3. Fix critical issues (Phase 1)
   4. Run audit again to verify
   5. Fix important issues (Phase 2)
   6. Manual testing with assistive technology
   7. Final audit before release
   ```

7. **Provide Action Items**

   Create actionable checklist:
   - [ ] Fix critical issue #1
   - [ ] Fix critical issue #2
   - [ ] Fix important issue #1
   - [ ] Test with keyboard only
   - [ ] Test with screen reader
   - [ ] Re-run audit
   - [ ] Verify WCAG AA compliance

## Usage Examples:

**Full codebase audit:**
```
/accessibility-checker:audit-a11y all
```

**Audit recent changes (default):**
```
/accessibility-checker:audit-a11y
# or
/accessibility-checker:audit-a11y changes
```

**Audit specific directory:**
```
/accessibility-checker:audit-a11y src/components
/accessibility-checker:audit-a11y src/pages
```

**Audit specific file:**
```
/accessibility-checker:audit-a11y src/components/Modal.tsx
```

## When to Run Audit:

### Before Major Releases
- Full codebase audit
- Ensure WCAG AA compliance
- Document any exceptions

### Before Creating PR
- Audit changed files
- Fix all critical issues
- Address important issues

### After Framework Upgrade
- Check for breaking changes
- Verify accessibility patterns still work
- Update deprecated patterns

### Quarterly Review
- Audit entire codebase
- Identify tech debt
- Plan remediation sprints

## Audit Best Practices:

1. **Fix in Priority Order**: Critical → Important → Advisory
2. **Re-audit After Fixes**: Verify issues are resolved
3. **Test Manually**: Automated audits can't catch everything
4. **Document Decisions**: Explain any deferred fixes
5. **Track Progress**: Monitor improvement over time
6. **Educate Team**: Share findings and learnings

## Integration with CI/CD:

Consider running automated checks:

```yaml
# .github/workflows/accessibility.yml
name: Accessibility Audit
on: [pull_request]
jobs:
  audit:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      - name: Run accessibility audit
        run: |
          # Run automated checks
          # Fail if critical issues found
```

## Compliance Levels:

**Level A** (Minimum):
- Basic accessibility
- Critical barriers removed

**Level AA** (Recommended):
- Enhanced accessibility
- Industry standard
- Legal compliance in many jurisdictions

**Level AAA** (Gold Standard):
- Highest accessibility
- Exceeds typical requirements

This audit targets **Level AA** compliance.

## Note:

This audit is comprehensive but automated analysis has limitations:

**Can Detect:**
- Missing ARIA attributes
- Semantic HTML issues
- Missing alt text
- Form label associations
- Code-level patterns

**Cannot Detect:**
- Alt text quality/meaningfulness
- Logical heading order in context
- Actual color contrast (need visual check)
- UX flow issues
- Context-dependent problems

**Always supplement with:**
- Manual testing
- Real screen reader testing
- User testing with people with disabilities

---

**Running this audit demonstrates commitment to digital inclusion. Every fix makes your product accessible to more users. 🌐♿**
