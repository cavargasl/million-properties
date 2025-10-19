import { PropertiesList } from '@/presentation/components/ui';

export default function PropertiesPage() {
  return (
    <div className="flex flex-col min-h-[calc(100vh-4rem)]">
      <main className="flex-1">
        <div className="container max-w-screen-2xl mx-auto py-8">
          {/* Page Header */}
          <div className="mb-8">
            <h1 className="text-4xl font-bold text-gray-900 mb-2">Propiedades</h1>
            <p className="text-lg text-gray-600">
              Explora nuestro catálogo completo de propiedades disponibles
            </p>
          </div>

          {/* Properties List with Pagination */}
          <PropertiesList />
        </div>
      </main>
    </div>
  );
}
