export const validateLoginForm = (formData) => {
  const errors = {}
  
  const symbols = [
    "$", "_", "#", "=", "+", "-", "*", "/", "%", "**", 
    "+", "-", "=", ">", "<", "&", "|", "!", "?", ":", "{", 
    "}", ",", ";", ".", "\"", "'", "`", "^", "~", "@"]

  const containsSymbol = (str) => symbols.some((symbol) => str.includes(symbol))
  const containsUppercase = (str) => /[A-Z]/.test(str)

  if (!formData.username || !formData.username.trim()) {
    errors.username = 'Username is required'
  }
  else if (formData.username.length < 3) {
    errors.username = 'Username must be at least 3 characters'
  }
  else if (formData.username.length > 20) {
    errors.username = 'Username cannot exceed 20 characters'
  }
  else if (containsSymbol(formData.username)) {
    errors.username = 'Username cannot contain special symbols'
  }

  if (!formData.password) {
    errors.password = 'Password is required'
  }
  else if (formData.password.length < 6) {
    errors.password = 'Password must be at least 6 characters'
  }
  else if (!containsUppercase(formData.password)) {
    errors.password = 'Password must contain at least one uppercase letter'
  }
  else if (!containsSymbol(formData.password)) {
    errors.password = 'Password must contain at least one special symbol' 
  }

  return errors
}