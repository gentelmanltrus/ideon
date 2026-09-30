export const validateLoginForm = (formData) => {
  const errors = {}

  if (!formData.username.trim()) {
    errors.username = 'Username is required'
  }
  else if (formData.username.length < 3) {
    errors.username = 'Username must be at least 3 characters'
  }

  if (!formData.password) {
    errors.password = 'Password is required'
  } 
  else if (formData.password.length < 6) {
    errors.password = 'Password must be at least 6 characters'
  }

  return errors
}