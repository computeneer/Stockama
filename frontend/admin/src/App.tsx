import { useEffect, useState } from "react";
import type { FormEvent } from "react";
import { ADMIN_API_BASE_URL, useAdminAuthStore } from "./auth";

function App() {
	const accessToken = useAdminAuthStore((state) => state.accessToken);
	const isAuthenticated = useAdminAuthStore((state) => state.isAuthenticated);
	const isInitializing = useAdminAuthStore((state) => state.isInitializing);
	const login = useAdminAuthStore((state) => state.login);
	const logout = useAdminAuthStore((state) => state.logout);
	const initializeSession = useAdminAuthStore((state) => state.initializeSession);

	const [username, setUsername] = useState("");
	const [password, setPassword] = useState("");
	const [isSubmitting, setIsSubmitting] = useState(false);
	const [error, setError] = useState<string | null>(null);

	useEffect(() => {
		void initializeSession();
	}, [initializeSession]);

	const handleLogin = async (event: FormEvent<HTMLFormElement>) => {
		event.preventDefault();
		setIsSubmitting(true);
		setError(null);

		try {
			await login(username, password);
		} catch (err) {
			const errorMessage = err instanceof Error ? err.message : "Login failed";
			setError(errorMessage);
		} finally {
			setIsSubmitting(false);
		}
	};

	const handleLogout = async () => {
		setError(null);
		await logout();
	};

	const shortToken = accessToken;

	return (
		<main className="mx-auto flex min-h-screen w-full max-w-xl items-center justify-center p-6">
			<section className="w-full rounded-xl border border-slate-300 bg-white p-6 shadow-sm">
				<h1 className="text-xl font-semibold">Admin Login Test</h1>
				<p className="mt-1 text-sm text-slate-600">API: {ADMIN_API_BASE_URL}</p>

				<div className="mt-4 rounded-md bg-slate-100 p-3 text-sm">
					<div>Initializing: {isInitializing ? "yes" : "no"}</div>
					<div>Authenticated: {isAuthenticated ? "yes" : "no"}</div>
					<div className="break-all">Access Token: {shortToken}</div>
				</div>

				{!isAuthenticated ? (
					<form onSubmit={handleLogin} className="mt-4 space-y-3">
						<input
							className="w-full rounded border border-slate-300 px-3 py-2"
							placeholder="Username"
							value={username}
							onChange={(event) => setUsername(event.target.value)}
						/>
						<input
							className="w-full rounded border border-slate-300 px-3 py-2"
							placeholder="Password"
							type="password"
							value={password}
							onChange={(event) => setPassword(event.target.value)}
						/>
						<button
							className="w-full rounded bg-slate-900 px-3 py-2 text-white disabled:opacity-50"
							type="submit"
							disabled={isSubmitting || isInitializing}>
							{isSubmitting ? "Logging in..." : "Login"}
						</button>
					</form>
				) : (
					<div className="mt-4 flex gap-2">
						<button
							className="rounded bg-slate-900 px-3 py-2 text-sm text-white"
							type="button"
							onClick={() => void initializeSession()}>
							Validate/Refresh
						</button>
						<button
							className="rounded bg-slate-700 px-3 py-2 text-sm text-white"
							type="button"
							onClick={() => void handleLogout()}>
							Logout
						</button>
					</div>
				)}

				{error ? <p className="mt-3 text-sm text-red-600">{error}</p> : null}
			</section>
		</main>
	);
}

export default App;
