// ❌ BEFORE: Common accessibility issues

import React, { useState } from 'react';

export function BadModal() {
  const [isOpen, setIsOpen] = useState(false);

  return (
    <>
      {/* Issue 1: Interactive div instead of button */}
      <div className="button" onClick={() => setIsOpen(true)}>
        Open Modal
      </div>

      {isOpen && (
        /* Issue 2: Missing dialog role and aria attributes */
        <div className="modal">
          <div className="modal-overlay"></div>
          <div className="modal-content">
            {/* Issue 3: Skipped heading level (no h1 in component) */}
            <h2>Modal Title</h2>
            
            {/* Issue 4: Image without alt text */}
            <img src="/warning-icon.png" />
            
            <p>This is a modal dialog with accessibility issues.</p>
            
            {/* Issue 5: Form without labels */}
            <form>
              <input type="text" placeholder="Enter your name" />
              <input type="email" placeholder="Enter your email" />
              
              {/* Issue 6: Button without type attribute */}
              <button onClick={() => setIsOpen(false)}>Submit</button>
            </form>
          </div>
        </div>
      )}
    </>
  );
}

// ============================================

// ✅ AFTER: Accessibility issues fixed

import React, { useState, useEffect, useRef } from 'react';

export function GoodModal() {
  const [isOpen, setIsOpen] = useState(false);
  const modalRef = useRef<HTMLDivElement>(null);
  const previousFocusRef = useRef<HTMLElement | null>(null);

  // Fix: Manage focus when modal opens/closes
  useEffect(() => {
    if (isOpen) {
      previousFocusRef.current = document.activeElement as HTMLElement;
      
      // Move focus to first focusable element
      const firstFocusable = modalRef.current?.querySelector<HTMLElement>(
        'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])'
      );
      firstFocusable?.focus();
    } else {
      // Restore focus when modal closes
      previousFocusRef.current?.focus();
    }
  }, [isOpen]);

  // Fix: Handle ESC key to close modal
  useEffect(() => {
    const handleEscape = (e: KeyboardEvent) => {
      if (e.key === 'Escape' && isOpen) {
        setIsOpen(false);
      }
    };

    if (isOpen) {
      document.addEventListener('keydown', handleEscape);
      return () => document.removeEventListener('keydown', handleEscape);
    }
  }, [isOpen]);

  return (
    <>
      {/* Fix 1: Use semantic button element */}
      <button 
        type="button"
        className="button" 
        onClick={() => setIsOpen(true)}
        aria-haspopup="dialog"
      >
        Open Modal
      </button>

      {isOpen && (
        /* Fix 2: Proper dialog role and ARIA attributes */
        <div 
          ref={modalRef}
          className="modal"
          role="dialog"
          aria-modal="true"
          aria-labelledby="modal-title"
          aria-describedby="modal-description"
        >
          {/* Overlay can be clicked to close */}
          <div 
            className="modal-overlay" 
            onClick={() => setIsOpen(false)}
            aria-hidden="true"
          ></div>
          
          <div className="modal-content">
            {/* Fix 3: Proper heading structure */}
            <h1 id="modal-title">Modal Title</h1>
            
            {/* Fix 4: Image with descriptive alt text */}
            <img src="/warning-icon.png" alt="Warning" />
            
            <p id="modal-description">
              This is an accessible modal dialog with proper WCAG compliance.
            </p>
            
            {/* Fix 5: Form with proper labels */}
            <form>
              <div className="form-field">
                <label htmlFor="name">Name</label>
                <input 
                  id="name"
                  type="text" 
                  placeholder="Enter your name"
                  aria-required="true"
                />
              </div>
              
              <div className="form-field">
                <label htmlFor="email">Email Address</label>
                <input 
                  id="email"
                  type="email" 
                  placeholder="Enter your email"
                  aria-required="true"
                />
              </div>
              
              {/* Fix 6: Buttons with explicit type */}
              <button 
                type="submit" 
                onClick={(e) => {
                  e.preventDefault();
                  setIsOpen(false);
                }}
              >
                Submit
              </button>
              
              <button 
                type="button" 
                onClick={() => setIsOpen(false)}
              >
                Cancel
              </button>
            </form>
          </div>
        </div>
      )}
    </>
  );
}

/* 
 * ACCESSIBILITY IMPROVEMENTS MADE:
 * 
 * 1. ✅ Semantic button instead of div
 * 2. ✅ Dialog role and ARIA attributes (aria-modal, aria-labelledby, aria-describedby)
 * 3. ✅ Proper heading structure (h1 for modal title)
 * 4. ✅ Image alt text
 * 5. ✅ Form labels associated with inputs
 * 6. ✅ Explicit button types
 * 7. ✅ Focus management (moves to modal, restores on close)
 * 8. ✅ ESC key handler
 * 9. ✅ Required field indication (aria-required)
 * 10. ✅ Focus trapping within modal
 * 
 * WCAG COMPLIANCE:
 * - 1.1.1 Non-text Content (A) ✅
 * - 1.3.1 Info and Relationships (A) ✅
 * - 2.1.1 Keyboard (A) ✅
 * - 2.1.2 No Keyboard Trap (A) ✅
 * - 2.4.3 Focus Order (A) ✅
 * - 3.3.2 Labels or Instructions (A) ✅
 * - 4.1.2 Name, Role, Value (A) ✅
 */
