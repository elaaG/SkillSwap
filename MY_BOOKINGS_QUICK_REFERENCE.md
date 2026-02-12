# 🚀 Quick Reference - My Bookings Feature

## 📞 API Endpoints Utilisés

### GET - Récupérer mes bookings
```http
GET /api/bookings/my
Authorization: Bearer {token}
```

**Réponse** :
```json
[
  {
    "bookingId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "state": "Pending",
    "escrowStatus": "Hold"
  }
]
```

### POST - Accepter un booking
```http
POST /api/bookings/{id}/accept
Authorization: Bearer {token}
```

**Réponse** : `200 OK`

### POST - Compléter un booking
```http
POST /api/bookings/{id}/complete
Authorization: Bearer {token}
```

**Réponse** : `200 OK`

### POST - Rejeter un booking
```http
POST /api/bookings/{id}/reject
Authorization: Bearer {token}
```

**Réponse** : `200 OK`

---

## 💻 Exemples d'Utilisation

### Utiliser BookingService dans un Composant

```razor
@page "/example"
@inject IBookingService BookingService

<button @onclick="LoadBookings">Load Bookings</button>

@code {
    private List<BookingDto> bookings = new();
    
    private async Task LoadBookings()
    {
        var result = await BookingService.GetMyBookingsAsync();
        
        if (result.Success && result.Data != null)
        {
            bookings = result.Data;
        }
        else
        {
            // Handle error
            Console.WriteLine(result.ErrorMessage);
        }
    }
}
```

### Afficher un BookingCard

```razor
<BookingCard 
    Booking="@myBooking" 
    OnActionCompleted="@RefreshBookings" />

@code {
    private BookingDto myBooking = new()
    {
        BookingId = Guid.NewGuid(),
        State = "Pending",
        EscrowStatus = "Hold"
    };
    
    private async Task RefreshBookings()
    {
        // Reload data after action
        await LoadBookingsAsync();
    }
}
```

### Utiliser les Badges Standalone

```razor
@* Afficher le statut du booking *@
<BookingStatusBadge State="Pending" />
<BookingStatusBadge State="Accepted" />
<BookingStatusBadge State="Completed" />
<BookingStatusBadge State="Rejected" />

@* Afficher le statut escrow *@
<EscrowStatusBadge Status="Hold" />
<EscrowStatusBadge Status="Released" />
<EscrowStatusBadge Status="Refunded" />
```

---

## 🎨 Exemples de Styles Personnalisés

### Modifier les Couleurs des Badges

Dans `BookingStatusBadge.razor` :

```css
.badge-pending {
    background-color: #fef3c7;  /* Changez cette valeur */
    color: #92400e;              /* Et celle-ci */
    border: 1px solid #fbbf24;   /* Et celle-là */
}
```

### Ajouter une Animation Personnalisée

Dans `BookingCard.razor` :

```css
.booking-card {
    /* ...styles existants... */
    animation: fadeIn 0.3s ease-in;
}

@keyframes fadeIn {
    from {
        opacity: 0;
        transform: translateY(10px);
    }
    to {
        opacity: 1;
        transform: translateY(0);
    }
}
```

### Changer la Grille

Dans `MyBookings.razor` :

```css
.bookings-grid {
    /* Par défaut : auto-fill, minmax(320px, 1fr) */
    
    /* Pour 2 colonnes fixes : */
    grid-template-columns: repeat(2, 1fr);
    
    /* Pour 4 colonnes sur grand écran : */
    grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
}
```

---

## 🔧 Gestion d'Erreurs Avancée

### Dans un Composant

```razor
@code {
    private string errorMessage = string.Empty;
    private bool hasError = false;
    
    private async Task AcceptBookingAsync(Guid bookingId)
    {
        try
        {
            var result = await BookingService.AcceptBookingAsync(bookingId);
            
            if (result.Success)
            {
                // Succès
                await OnActionCompleted.InvokeAsync();
                hasError = false;
                errorMessage = string.Empty;
            }
            else
            {
                // Erreur métier
                hasError = true;
                errorMessage = result.ErrorMessage ?? "An error occurred";
            }
        }
        catch (HttpRequestException ex)
        {
            // Erreur réseau
            hasError = true;
            errorMessage = "Network error. Please check your connection.";
        }
        catch (Exception ex)
        {
            // Erreur inattendue
            hasError = true;
            errorMessage = "An unexpected error occurred.";
            Console.Error.WriteLine($"Error: {ex.Message}");
        }
    }
}
```

---

## 🎯 Tests Manuels - Checklist

### Test 1 : Chargement Initial
- [ ] Naviguer vers `/my-bookings`
- [ ] Vérifier que le spinner s'affiche
- [ ] Vérifier que les bookings se chargent
- [ ] Vérifier que les statistiques sont correctes

### Test 2 : Accepter un Booking
- [ ] Identifier un booking avec statut "Pending"
- [ ] Cliquer sur le bouton "Accept"
- [ ] Vérifier que le spinner s'affiche dans le bouton
- [ ] Vérifier que le statut passe à "Accepted"
- [ ] Vérifier que le bouton "Complete" apparaît

### Test 3 : Compléter un Booking
- [ ] Identifier un booking avec statut "Accepted"
- [ ] Cliquer sur le bouton "Complete"
- [ ] Vérifier que le statut passe à "Completed"
- [ ] Vérifier que l'escrow status passe à "Released"
- [ ] Vérifier que les boutons disparaissent

### Test 4 : Rejeter un Booking
- [ ] Identifier un booking avec statut "Pending"
- [ ] Cliquer sur le bouton "Reject"
- [ ] Vérifier que le statut passe à "Rejected"
- [ ] Vérifier que l'escrow status passe à "Refunded"
- [ ] Vérifier que les boutons disparaissent

### Test 5 : État Vide
- [ ] Se connecter avec un compte sans bookings
- [ ] Vérifier l'affichage de l'état vide
- [ ] Vérifier le bouton "Browse Services"

### Test 6 : Gestion d'Erreurs
- [ ] Arrêter le backend
- [ ] Rafraîchir la page
- [ ] Vérifier l'affichage du message d'erreur
- [ ] Cliquer sur "Retry"
- [ ] Redémarrer le backend
- [ ] Cliquer à nouveau sur "Retry"

### Test 7 : Responsive Design
- [ ] Tester sur écran desktop (> 768px)
- [ ] Tester sur tablette (768px)
- [ ] Tester sur mobile (< 480px)
- [ ] Vérifier que la grille s'adapte
- [ ] Vérifier que les boutons s'empilent sur mobile

---

## 📝 Snippets Utiles

### Créer un Booking de Test (Backend)

```csharp
// Dans un controller ou une seed method
var testBooking = new Booking
{
    ClientId = "user-id-here",
    ProviderId = "provider-id-here",
    ListingId = Guid.NewGuid(),
    StartTime = DateTime.UtcNow.AddDays(1),
    EndTime = DateTime.UtcNow.AddDays(1).AddHours(2),
    State = BookingState.Pending,
    Escrow = new EscrowTransaction
    {
        Amount = 50.00m,
        Status = EscrowStatus.Hold
    }
};

_context.Bookings.Add(testBooking);
await _context.SaveChangesAsync();
```

### Ajouter un Log de Débogage

```csharp
// Dans BookingService.cs
public async Task<ApiResponse<List<BookingDto>>> GetMyBookingsAsync()
{
    Console.WriteLine("[BookingService] Fetching bookings...");
    
    try
    {
        var bookings = await _httpClient.GetFromJsonAsync<List<BookingDto>>("api/bookings/my");
        
        Console.WriteLine($"[BookingService] Received {bookings?.Count ?? 0} bookings");
        
        // ...reste du code
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"[BookingService] Error: {ex.Message}");
        throw;
    }
}
```

### Simuler un Délai (Pour Tester le Loading)

```csharp
// Dans BookingService.cs (UNIQUEMENT POUR TESTS)
public async Task<ApiResponse<List<BookingDto>>> GetMyBookingsAsync()
{
    await Task.Delay(2000); // Délai de 2 secondes
    
    // ...reste du code
}
```

---

## 🎨 Palette de Couleurs Complète

### Primary Colors
```css
--blue-50: #eff6ff;
--blue-100: #dbeafe;
--blue-500: #3b82f6;
--blue-600: #2563eb;
--blue-700: #1d4ed8;
```

### Success Colors
```css
--green-50: #f0fdf4;
--green-100: #dcfce7;
--green-500: #22c55e;
--green-600: #16a34a;
--green-700: #15803d;
```

### Warning Colors
```css
--yellow-50: #fefce8;
--yellow-100: #fef3c7;
--yellow-500: #eab308;
--yellow-600: #ca8a04;
--yellow-700: #a16207;
```

### Error Colors
```css
--red-50: #fef2f2;
--red-100: #fecaca;
--red-500: #ef4444;
--red-600: #dc2626;
--red-700: #b91c1c;
```

### Neutral Colors
```css
--gray-50: #f9fafb;
--gray-100: #f3f4f6;
--gray-500: #6b7280;
--gray-700: #374151;
--gray-900: #111827;
```

---

## 🔗 Liens Utiles

### Navigation
```razor
@* Lien vers My Bookings depuis un autre composant *@
<a href="/my-bookings">View My Bookings</a>

@* Navigation programmatique *@
@inject NavigationManager NavManager

@code {
    private void GoToBookings()
    {
        NavManager.NavigateTo("/my-bookings");
    }
}
```

### Ajout au Menu Principal

Dans `MainLayout.razor` ou `NavMenu.razor` :

```razor
<NavLink class="nav-link" href="my-bookings">
    <span class="icon">📅</span>
    <span class="text">My Bookings</span>
</NavLink>
```

---

## 🐛 Debugging Tips

### Console du Navigateur
```javascript
// Voir les requêtes réseau
// Ouvrir DevTools > Network > Filter: /api/bookings
```

### Logs Backend
```bash
# Dans SkillSwap.Api
dotnet run --verbosity detailed
```

### Breakpoints dans Blazor
```csharp
@code {
    private async Task LoadBookingsAsync()
    {
        System.Diagnostics.Debugger.Break(); // Pause ici
        
        var result = await BookingService.GetMyBookingsAsync();
        // ...
    }
}
```

---

## ✨ Bonnes Pratiques

### 1. Toujours Gérer les Erreurs
```csharp
✅ DO:
try {
    var result = await service.GetDataAsync();
    if (result.Success) { /* ... */ }
    else { /* Handle error */ }
} catch { /* Handle exception */ }

❌ DON'T:
var result = await service.GetDataAsync();
var data = result.Data; // Peut être null !
```

### 2. Utiliser StateHasChanged() Judicieusement
```csharp
✅ DO:
IsLoading = true;
StateHasChanged(); // UI se met à jour immédiatement

await LoadDataAsync();

IsLoading = false;
StateHasChanged(); // UI se met à jour après chargement

❌ DON'T:
// Appeler StateHasChanged() dans une boucle
foreach (var item in items) {
    StateHasChanged(); // Mauvaise performance !
}
```

### 3. Disposer les Ressources
```csharp
@implements IDisposable

@code {
    private CancellationTokenSource? cts;
    
    protected override async Task OnInitializedAsync()
    {
        cts = new CancellationTokenSource();
        await LoadDataAsync(cts.Token);
    }
    
    public void Dispose()
    {
        cts?.Cancel();
        cts?.Dispose();
    }
}
```

---

## 🎓 Ressources Supplémentaires

### Documentation Microsoft
- [Blazor WebAssembly](https://learn.microsoft.com/en-us/aspnet/core/blazor/)
- [HttpClient in Blazor](https://learn.microsoft.com/en-us/aspnet/core/blazor/call-web-api)
- [Component Lifecycle](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/lifecycle)

### Tutoriels
- [Blazor Authentication](https://learn.microsoft.com/en-us/aspnet/core/blazor/security/)
- [State Management](https://learn.microsoft.com/en-us/aspnet/core/blazor/state-management)

---

**🎉 Vous êtes maintenant prêt à utiliser et personnaliser My Bookings !**
