// ❌ BEFORE: Form with accessibility barriers

import React, { useState } from 'react';

export function BadForm() {
  const [errors, setErrors] = useState<Record<string, string>>({});

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    // Validation logic
  };

  return (
    <div className="form-container">
      <div className="form-title">Sign Up</div>
      
      <form onSubmit={handleSubmit}>
        {/* Issue 1: No fieldset/legend for grouped fields */}
        {/* Issue 2: Inputs without labels */}
        <input type="text" name="firstName" placeholder="First Name" />
        <input type="text" name="lastName" placeholder="Last Name" />
        
        {/* Issue 3: Email without label */}
        <input type="email" name="email" placeholder="Email *" />
        
        {/* Issue 4: Password without label or requirements */}
        <input type="password" name="password" placeholder="Password *" />
        
        {/* Issue 5: Checkboxes without proper labels */}
        <div>
          <input type="checkbox" name="newsletter" id="news" />
          <span onClick={() => document.getElementById('news')?.click()}>
            Subscribe to newsletter
          </span>
        </div>
        
        <div>
          <input type="checkbox" name="terms" id="terms" />
          <span onClick={() => document.getElementById('terms')?.click()}>
            I agree to terms *
          </span>
        </div>
        
        {/* Issue 6: Error messages not announced */}
        {errors.email && (
          <div className="error" style={{ color: 'red' }}>
            {errors.email}
          </div>
        )}
        
        {/* Issue 7: Submit button without proper type */}
        <div className="submit-btn" onClick={handleSubmit}>
          Submit
        </div>
      </form>
    </div>
  );
}

// ============================================

// ✅ AFTER: Accessible form

import React, { useState } from 'react';

export function GoodForm() {
  const [errors, setErrors] = useState<Record<string, string>>({});

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    // Validation logic
  };

  return (
    <div className="form-container">
      {/* Fix: Use semantic heading */}
      <h1>Sign Up</h1>
      
      <form onSubmit={handleSubmit} noValidate>
        {/* Fix 1: Fieldset and legend for grouped fields */}
        <fieldset>
          <legend>Personal Information</legend>
          
          {/* Fix 2: Proper labels associated with inputs */}
          <div className="form-field">
            <label htmlFor="firstName">
              First Name
              <span aria-label="required">*</span>
            </label>
            <input 
              type="text" 
              id="firstName"
              name="firstName" 
              placeholder="First Name"
              required
              aria-required="true"
            />
          </div>
          
          <div className="form-field">
            <label htmlFor="lastName">
              Last Name
              <span aria-label="required">*</span>
            </label>
            <input 
              type="text" 
              id="lastName"
              name="lastName" 
              placeholder="Last Name"
              required
              aria-required="true"
            />
          </div>
        </fieldset>
        
        <fieldset>
          <legend>Account Details</legend>
          
          {/* Fix 3: Email with proper label and error handling */}
          <div className="form-field">
            <label htmlFor="email">
              Email Address
              <span aria-label="required">*</span>
            </label>
            <input 
              type="email" 
              id="email"
              name="email" 
              placeholder="email@example.com"
              required
              aria-required="true"
              aria-invalid={!!errors.email}
              aria-describedby={errors.email ? "email-error" : undefined}
            />
            {/* Fix 6: Error message with proper ARIA */}
            {errors.email && (
              <div 
                id="email-error"
                className="error" 
                role="alert"
                aria-live="polite"
              >
                <span className="error-icon" aria-hidden="true">⚠</span>
                {errors.email}
              </div>
            )}
          </div>
          
          {/* Fix 4: Password with label and requirements description */}
          <div className="form-field">
            <label htmlFor="password">
              Password
              <span aria-label="required">*</span>
            </label>
            <input 
              type="password" 
              id="password"
              name="password" 
              placeholder="Enter password"
              required
              aria-required="true"
              aria-describedby="password-requirements"
            />
            <div id="password-requirements" className="help-text">
              Must be at least 8 characters with one uppercase letter, 
              one number, and one special character.
            </div>
          </div>
        </fieldset>
        
        {/* Fix 5: Checkboxes with proper label elements */}
        <fieldset>
          <legend>Preferences</legend>
          
          <div className="form-field">
            <label htmlFor="newsletter" className="checkbox-label">
              <input 
                type="checkbox" 
                id="newsletter"
                name="newsletter"
              />
              Subscribe to newsletter
            </label>
          </div>
          
          <div className="form-field">
            <label htmlFor="terms" className="checkbox-label">
              <input 
                type="checkbox" 
                id="terms"
                name="terms"
                required
                aria-required="true"
              />
              I agree to the <a href="/terms">terms and conditions</a>
              <span aria-label="required">*</span>
            </label>
          </div>
        </fieldset>
        
        {/* Fix 7: Semantic submit button */}
        <button 
          type="submit" 
          className="submit-btn"
        >
          Submit
        </button>
      </form>
    </div>
  );
}

/* 
 * ACCESSIBILITY IMPROVEMENTS MADE:
 * 
 * 1. ✅ Semantic heading (h1) instead of div
 * 2. ✅ Fieldsets and legends for grouped fields
 * 3. ✅ Labels properly associated with all inputs
 * 4. ✅ Required fields indicated with aria-required
 * 5. ✅ Error messages with role="alert" and aria-live
 * 6. ✅ aria-invalid on fields with errors
 * 7. ✅ aria-describedby linking errors and help text
 * 8. ✅ Semantic button for submit
 * 9. ✅ Proper checkbox label association
 * 10. ✅ Password requirements described
 * 
 * WCAG COMPLIANCE:
 * - 1.3.1 Info and Relationships (A) ✅
 * - 3.2.2 On Input (A) ✅
 * - 3.3.1 Error Identification (A) ✅
 * - 3.3.2 Labels or Instructions (A) ✅
 * - 3.3.3 Error Suggestion (AA) ✅
 * - 4.1.2 Name, Role, Value (A) ✅
 * - 4.1.3 Status Messages (AA) ✅
 */
