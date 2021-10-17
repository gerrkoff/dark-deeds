const baseUrl =
  process.env.NODE_ENV === 'production'
    ? '/'
    : `http://${window.location.hostname}:5000/`

export default baseUrl
