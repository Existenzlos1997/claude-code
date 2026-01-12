#!/usr/bin/env python3
"""
Simple test script to verify the accessibility hook works correctly.
"""

import json
import os
import subprocess
import sys


def clear_test_state():
    """Clear any existing test state."""
    state_file = os.path.expanduser("~/.claude/a11y_warnings_state_test-session.json")
    if os.path.exists(state_file):
        os.remove(state_file)


def test_hook(test_name, tool_input, expected_exit_code, should_contain_text=None):
    """Test the accessibility hook with given input."""
    print(f"\n{'='*60}")
    print(f"Test: {test_name}")
    print(f"{'='*60}")
    
    # Clear state before each test
    clear_test_state()
    
    hook_path = "/home/runner/work/claude-code/claude-code/plugins/accessibility-checker/hooks/accessibility_hook.py"
    
    # Prepare input
    input_data = {
        "session_id": "test-session",
        "tool_name": "Write",
        "tool_input": tool_input
    }
    
    # Run the hook
    try:
        result = subprocess.run(
            ["python3", hook_path],
            input=json.dumps(input_data),
            capture_output=True,
            text=True,
            timeout=5
        )
        
        print(f"Exit code: {result.returncode} (expected: {expected_exit_code})")
        
        if result.stderr:
            print(f"\nStderr output:")
            print(result.stderr)
        
        # Check exit code
        if result.returncode != expected_exit_code:
            print(f"❌ FAILED: Expected exit code {expected_exit_code}, got {result.returncode}")
            return False
        
        # Check for expected text in stderr
        if should_contain_text and should_contain_text not in result.stderr:
            print(f"❌ FAILED: Expected text '{should_contain_text}' not found in stderr")
            return False
        
        print(f"✅ PASSED")
        return True
        
    except Exception as e:
        print(f"❌ ERROR: {e}")
        return False


def main():
    """Run all tests."""
    print("Testing Accessibility Hook")
    print("="*60)
    
    results = []
    
    # Test 1: Interactive div (should trigger warning)
    results.append(test_hook(
        "Interactive div without keyboard support",
        {
            "file_path": "test.tsx",
            "content": '<div onClick={handleClick}>Click me</div>'
        },
        expected_exit_code=2,
        should_contain_text="Interactive div/span"
    ))
    
    # Test 2: Image without alt (should trigger warning)
    results.append(test_hook(
        "Image without alt text",
        {
            "file_path": "test.tsx",
            "content": '<img src="logo.png" />'
        },
        expected_exit_code=2,
        should_contain_text="missing alt text"
    ))
    
    # Test 3: Input without label (should trigger warning)
    results.append(test_hook(
        "Input without label",
        {
            "file_path": "test.tsx",
            "content": '<input type="email" placeholder="Email" />'
        },
        expected_exit_code=2,
        should_contain_text="missing accessible label"
    ))
    
    # Test 4: Redundant button role (should trigger warning)
    results.append(test_hook(
        "Redundant ARIA role on button",
        {
            "file_path": "test.tsx",
            "content": '<button role="button">Click</button>'
        },
        expected_exit_code=2,
        should_contain_text="Redundant ARIA role"
    ))
    
    # Test 5: Accessible button (should pass)
    results.append(test_hook(
        "Accessible button (should pass)",
        {
            "file_path": "test.tsx",
            "content": '<button type="button" onClick={handleClick}>Click me</button>'
        },
        expected_exit_code=0
    ))
    
    # Test 6: Accessible image (should pass)
    results.append(test_hook(
        "Image with alt text (should pass)",
        {
            "file_path": "test.tsx",
            "content": '<img src="logo.png" alt="Company Logo" />'
        },
        expected_exit_code=0
    ))
    
    # Test 7: Non-UI file (should pass)
    results.append(test_hook(
        "Non-UI file (should pass)",
        {
            "file_path": "test.py",
            "content": 'def hello():\n    print("Hello")'
        },
        expected_exit_code=0
    ))
    
    # Summary
    print(f"\n{'='*60}")
    print("Test Summary")
    print(f"{'='*60}")
    passed = sum(results)
    total = len(results)
    print(f"Passed: {passed}/{total}")
    
    if passed == total:
        print("✅ All tests passed!")
        return 0
    else:
        print(f"❌ {total - passed} test(s) failed")
        return 1


if __name__ == "__main__":
    sys.exit(main())
