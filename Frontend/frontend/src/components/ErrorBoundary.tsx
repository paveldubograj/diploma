import React from "react";

type State = { hasError: boolean };

class ErrorBoundary extends React.Component<React.PropsWithChildren, State> {
  constructor(props: {}) {
    super(props);
    this.state = { hasError: false };
  }

  static getDerivedStateFromError(_: Error): State {
    return { hasError: true };
  }

  componentDidCatch(error: Error, errorInfo: React.ErrorInfo) {
    console.error("Ошибка:", error, errorInfo);
  }

  render() {
    if (this.state.hasError) {
      return <h2>Что-то пошло не так.</h2>;
    }

    return this.props.children;
  }
}

export default ErrorBoundary;
