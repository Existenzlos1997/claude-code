// ❌ BEFORE: Navigation with accessibility issues

import React from 'react';

export function BadNavigation() {
  return (
    <div className="header">
      {/* Issue 1: Navigation wrapped in generic div */}
      <div className="nav">
        {/* Issue 2: Logo image without alt text */}
        <img src="/logo.png" className="logo" />
        
        {/* Issue 3: Clickable divs instead of links */}
        <div className="nav-items">
          <div className="nav-item" onClick={() => window.location.href = '/'}>
            Home
          </div>
          <div className="nav-item" onClick={() => window.location.href = '/about'}>
            About
          </div>
          <div className="nav-item" onClick={() => window.location.href = '/contact'}>
            Contact
          </div>
        </div>
        
        {/* Issue 4: Icon-only button without accessible label */}
        <div className="menu-toggle" onClick={() => alert('Toggle menu')}>
          <span className="icon">☰</span>
        </div>
      </div>
    </div>
  );
}

// ============================================

// ✅ AFTER: Accessible navigation

import React, { useState } from 'react';

export function GoodNavigation() {
  const [isMenuOpen, setIsMenuOpen] = useState(false);

  return (
    <header className="header">
      {/* Fix 1: Use semantic nav element with landmark role */}
      <nav aria-label="Main navigation">
        {/* Fix 2: Logo with descriptive alt text */}
        <a href="/" aria-label="Home">
          <img 
            src="/logo.png" 
            alt="Company Name Logo" 
            className="logo"
          />
        </a>
        
        {/* Fix 3: Use semantic links for navigation */}
        <ul className="nav-items">
          <li>
            <a href="/" aria-current="page">
              Home
            </a>
          </li>
          <li>
            <a href="/about">
              About
            </a>
          </li>
          <li>
            <a href="/contact">
              Contact
            </a>
          </li>
        </ul>
        
        {/* Fix 4: Button with accessible label and proper ARIA */}
        <button 
          type="button"
          className="menu-toggle"
          onClick={() => setIsMenuOpen(!isMenuOpen)}
          aria-label="Toggle navigation menu"
          aria-expanded={isMenuOpen}
          aria-controls="mobile-menu"
        >
          <span className="icon" aria-hidden="true">☰</span>
        </button>
      </nav>
      
      {/* Mobile menu with proper ARIA */}
      {isMenuOpen && (
        <div id="mobile-menu" className="mobile-menu">
          <ul>
            <li>
              <a href="/">Home</a>
            </li>
            <li>
              <a href="/about">About</a>
            </li>
            <li>
              <a href="/contact">Contact</a>
            </li>
          </ul>
        </div>
      )}
    </header>
  );
}

/* 
 * ACCESSIBILITY IMPROVEMENTS MADE:
 * 
 * 1. ✅ Semantic HTML elements (nav, header, ul, li, a)
 * 2. ✅ Logo image with descriptive alt text
 * 3. ✅ Links instead of clickable divs
 * 4. ✅ aria-current for current page
 * 5. ✅ aria-label for navigation landmark
 * 6. ✅ Button with aria-label for icon-only button
 * 7. ✅ aria-expanded state for toggle button
 * 8. ✅ aria-controls linking button to menu
 * 9. ✅ aria-hidden on decorative icon
 * 10. ✅ Proper button type attribute
 * 
 * WCAG COMPLIANCE:
 * - 1.1.1 Non-text Content (A) ✅
 * - 1.3.1 Info and Relationships (A) ✅
 * - 2.1.1 Keyboard (A) ✅
 * - 2.4.1 Bypass Blocks (A) ✅ (via nav landmark)
 * - 4.1.2 Name, Role, Value (A) ✅
 */
