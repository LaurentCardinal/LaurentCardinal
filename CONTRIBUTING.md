# Contributing to Kiosk Browser

Thank you for your interest in contributing to Kiosk Browser! This document provides guidelines and instructions for contributing.

## Code of Conduct

- Be respectful and inclusive
- Focus on constructive feedback
- Help create a welcoming environment for all contributors

## How to Contribute

### Reporting Bugs

1. Check if the bug has already been reported in Issues
2. If not, create a new issue with:
   - Clear, descriptive title
   - Steps to reproduce the bug
   - Expected behavior vs actual behavior
   - System information (Windows version, .NET version)
   - Screenshots if applicable

### Suggesting Features

1. Check if the feature has already been requested
2. Create a new issue describing:
   - The problem the feature would solve
   - How it would work
   - Why it would be useful for kiosk environments

### Submitting Code

1. **Fork the repository**
   ```bash
   git clone https://github.com/LaurentCardinal/LaurentCardinal.git
   cd LaurentCardinal
   ```

2. **Create a feature branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```

3. **Make your changes**
   - Follow the existing code style
   - Add comments for complex logic
   - Update documentation if needed

4. **Test your changes**
   - Build the project: `dotnet build`
   - Run the application: `dotnet run`
   - Test in fullscreen mode
   - Test virtual keyboard
   - Test admin mode features

5. **Commit your changes**
   ```bash
   git add .
   git commit -m "Add: brief description of your changes"
   ```

6. **Push to your fork**
   ```bash
   git push origin feature/your-feature-name
   ```

7. **Create a Pull Request**
   - Describe what changes you made and why
   - Reference any related issues
   - Include screenshots for UI changes

## Development Guidelines

### Code Style

- Use meaningful variable and method names
- Follow C# naming conventions:
  - PascalCase for classes, methods, properties
  - camelCase for local variables and parameters
  - _camelCase for private fields
- Add XML documentation comments for public APIs
- Keep methods focused and concise

### XAML Style

- Use consistent indentation (4 spaces)
- Group related properties together
- Use static resources for colors and styles
- Keep XAML readable and well-organized

### Testing Checklist

Before submitting a PR, ensure:

- [ ] Code compiles without warnings
- [ ] Application starts successfully
- [ ] Fullscreen mode works correctly
- [ ] Virtual keyboard functions properly
- [ ] Navigation buttons work as expected
- [ ] Admin mode can be enabled/disabled
- [ ] Settings can be saved and loaded
- [ ] Idle timeout works (if configured)
- [ ] Domain blocking works (if configured)
- [ ] No regression in existing features

### Commit Message Format

Use clear, descriptive commit messages:

```
Add: New feature description
Fix: Bug fix description
Update: Changes to existing feature
Refactor: Code restructuring
Docs: Documentation changes
Style: Formatting, missing semicolons, etc.
Test: Adding or updating tests
```

## Areas for Contribution

We welcome contributions in these areas:

### High Priority
- Multi-language support (i18n)
- Accessibility improvements
- Performance optimizations
- Security enhancements

### Features
- Session management
- Usage analytics
- Remote configuration
- Custom keyboard layouts
- Whitelist mode for domains

### UI/UX
- Theme customization
- Animation improvements
- Better touch support
- Accessibility features

### Documentation
- Translation to other languages
- Video tutorials
- Deployment guides
- Best practices documentation

## Questions?

Feel free to open an issue with the "question" label if you need help or clarification.

Thank you for contributing to Kiosk Browser!
