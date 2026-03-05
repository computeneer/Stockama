import { useMemo, useState } from "react";
import {
	AppButton,
	KeyValueList,
	Panel,
	ThemeToggleButton,
	createApiClient,
	createCompanyApi,
	useApiMutation,
	useApiQuery,
	useTheme,
	type CompanyListRequest,
	type CreateCompanyRequest,
} from "@stockama/shared";

const apiClient = createApiClient({
	baseUrl: import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5001",
});

const companyApi = createCompanyApi(apiClient);

function App() {
	const { theme, toggleTheme } = useTheme();
	const [lastCreatedCompany, setLastCreatedCompany] = useState<string | null>(null);

	const listRequest = useMemo<CompanyListRequest>(() => ({ page: 1, pageSize: 10 }), []);

	const companyQuery = useApiQuery(() => companyApi.list(listRequest), [listRequest]);

	const createCompanyMutation = useApiMutation<CreateCompanyRequest, Awaited<ReturnType<typeof companyApi.create>>>(
		(payload) => companyApi.create(payload),
	);

	async function handleCreateCompany() {
		const companyName = `AdminCompany-${Date.now()}`;
		try {
			await createCompanyMutation.mutate({ companyName });
			setLastCreatedCompany(companyName);
			await companyQuery.refetch();
		} catch {
			setLastCreatedCompany(null);
		}
	}

	return (
		<main className="mx-auto grid min-h-screen w-full max-w-5xl gap-4 px-4 py-8">
			<header className="flex flex-wrap items-center justify-between gap-3">
				<div>
					<h1 className="m-0 text-2xl font-bold tracking-tight">Stockama Admin</h1>
					<p className="m-0 mt-1 text-sm text-slate-600 dark:text-slate-400">
						Tailwind + shared hook/type/component yapisi aktif.
					</p>
				</div>
				<ThemeToggleButton theme={theme} onToggle={toggleTheme} />
			</header>

			<Panel
				title="Company Listesi"
				subtitle="request/response tipleri @stockama/shared/src/types/api/company altindan gelir.">
				<KeyValueList
					items={[
						{ key: "Theme", value: theme },
						{ key: "Durum", value: companyQuery.isLoading ? "Yukleniyor" : "Hazir" },
						{ key: "Toplam Kayit", value: companyQuery.data?.totalCount ?? companyQuery.data?.data.length ?? 0 },
						{ key: "Son Hata", value: companyQuery.error?.message ?? "-" },
					]}
				/>

				<div className="flex flex-wrap gap-2">
					<AppButton onClick={() => void companyQuery.refetch()} variant="secondary">
						Listeyi Yenile
					</AppButton>
					<AppButton onClick={() => void handleCreateCompany()} disabled={createCompanyMutation.isLoading}>
						Sirket Olustur
					</AppButton>
				</div>

				<p className="m-0 text-sm text-slate-600 dark:text-slate-400">
					Son olusturulan: {lastCreatedCompany ?? "-"} | Mutation hata:{" "}
					{createCompanyMutation.error?.message ?? "-"}
				</p>
			</Panel>
		</main>
	);
}

export default App;
