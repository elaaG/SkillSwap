# 📱 Guide d'Utilisation - My Bookings Feature

## 🎨 Fonctionnalités Implémentées

### ✅ Composants Créés

1. **BookingService.cs** (`SkillSwap.Client/Services/`)
   - Service pour interagir avec l'API Booking
   - Méthodes : GetMyBookings, Accept, Complete, Reject
   - Gestion d'erreurs complète avec ApiResponse<T>

2. **BookingModels.cs** (`SkillSwap.Client/Models/`)
   - BookingDto : modèle pour les données de booking
   - CreateBookingRequest : modèle pour créer un booking

3. **BookingStatusBadge.razor** (`SkillSwap.Client/Shared/`)
   - Badge coloré pour afficher l'état du booking
   - États : Pending (jaune), Accepted (bleu), Completed (vert), Rejected (rouge)

4. **EscrowStatusBadge.razor** (`SkillSwap.Client/Shared/`)
   - Badge avec icône pour le statut escrow
   - Statuts : Hold 🔒, Released ✅, Refunded ↩️

5. **BookingCard.razor** (`SkillSwap.Client/Shared/`)
   - Carte moderne pour afficher un booking
   - Actions conditionnelles basées sur l'état
   - Indicateurs de chargement et gestion d'erreurs

6. **MyBookings.razor** (`SkillSwap.Client/Pages/Booking/`)
   - Page principale pour gérer les bookings
   - Grille responsive de cartes
   - États : Loading, Empty, Error, Success
   - Statistiques en temps réel

---

## 🎯 Logique d'Affichage des Actions

### État "Pending" (En attente)
```
┌────────────────────────┐
│  📅 Booking Card       │
│  ━━━━━━━━━━━━━━━━━━━━│
│  ID: abc123...         │
│  Status: PENDING       │
│  Escrow: 🔒 Hold       │
│                        │
│  [✓ Accept] [✕ Reject] │
└────────────────────────┘
```

### État "Accepted" (Accepté)
```
┌────────────────────────┐
│  📅 Booking Card       │
│  ━━━━━━━━━━━━━━━━━━━━│
│  ID: abc123...         │
│  Status: ACCEPTED      │
│  Escrow: 🔒 Hold       │
│                        │
│  [✓ Complete]          │
└────────────────────────┘
```

### État "Completed" (Terminé)
```
┌────────────────────────┐
│  📅 Booking Card       │
│  ━━━━━━━━━━━━━━━━━━━━│
│  ID: abc123...         │
│  Status: COMPLETED     │
│  Escrow: ✅ Released   │
│                        │
│  (Aucune action)       │
└────────────────────────┘
```

### État "Rejected" (Rejeté)
```
┌────────────────────────┐
│  📅 Booking Card       │
│  ━━━━━━━━━━━━━━━━━━━━│
│  ID: abc123...         │
│  Status: REJECTED      │
│  Escrow: ↩️ Refunded   │
│                        │
│  (Aucune action)       │
└────────────────────────┘
```

---

## 🚀 Comment Tester

### 1. Prérequis
- API Backend en cours d'exécution sur `http://localhost:5001`
- Utilisateur authentifié avec token valide
- Au moins un booking existant dans la base de données

### 2. Accéder à la Page
Naviguez vers : **`https://localhost:7001/my-bookings`**

### 3. Scénarios de Test

#### Scénario A : Booking en Attente
1. Vous voyez une carte avec le statut **PENDING**
2. Deux boutons sont disponibles : **Accept** et **Reject**
3. Cliquez sur **Accept**
   - Le bouton affiche un spinner : "Accepting..."
   - Après succès, le statut passe à **ACCEPTED**
   - Le bouton **Complete** apparaît

#### Scénario B : Accepter puis Compléter
1. Booking avec statut **ACCEPTED**
2. Bouton **Complete** visible
3. Cliquez sur **Complete**
   - Spinner : "Completing..."
   - Après succès : statut → **COMPLETED**
   - Escrow status → **Released** ✅
   - Transfert d'argent effectué côté backend

#### Scénario C : Rejeter un Booking
1. Booking avec statut **PENDING**
2. Cliquez sur **Reject**
   - Spinner : "Rejecting..."
   - Après succès : statut → **REJECTED**
   - Escrow status → **Refunded** ↩️
   - Argent remboursé au client

#### Scénario D : Page Vide
1. Si aucun booking n'existe :
   - Icône 📅
   - Message : "No Bookings Yet"
   - Bouton : "Browse Services"

#### Scénario E : Erreur de Chargement
1. Si l'API est down :
   - Icône ⚠️
   - Message d'erreur
   - Bouton "🔄 Retry"

---

## 🎨 Design Features

### Couleurs Sémantiques
- **Pending** : Jaune/Orange (#fef3c7, #92400e)
- **Accepted** : Bleu (#bfdbfe, #1e3a8a)
- **Completed** : Vert (#bbf7d0, #14532d)
- **Rejected** : Rouge (#fecaca, #7f1d1d)

### Animations & Transitions
- ✨ Hover effects sur les cartes (translateY, shadow)
- 🔄 Spinners de chargement
- 🎯 Transitions fluides (0.2s ease)

### Responsive Design
- **Desktop** : Grille 3 colonnes
- **Tablet** : Grille 2 colonnes
- **Mobile** : 1 colonne
- Boutons en colonne sur petit écran

### Accessibilité
- Contraste des couleurs WCAG AA
- États disabled visuellement distincts
- Messages d'erreur clairs
- États de chargement explicites

---

## 📊 Barre de Statistiques

La page affiche un résumé en haut :

```
┌──────────────┬──────────────┬──────────────┬──────────────┐
│ Total        │ Pending      │ Accepted     │ Completed    │
│ Bookings     │              │              │              │
│              │              │              │              │
│   12         │    3         │    2         │    7         │
└──────────────┴──────────────┴──────────────┴──────────────┘
```

Les statistiques sont automatiquement calculées et mises à jour.

---

## 🔧 Gestion d'État

### Loading States
```csharp
IsLoading = true → Affiche spinner
IsLoading = false → Affiche contenu
```

### Processing States (Boutons)
```csharp
IsProcessing = true → Boutons disabled + spinner
IsProcessing = false → Boutons actifs
```

### Error Handling
```csharp
ErrorMessage != null → Affiche message d'erreur
ErrorMessage == null → Pas d'erreur
```

---

## 📝 Code Structure

### BookingService.cs
```csharp
public interface IBookingService
{
    Task<ApiResponse<List<BookingDto>>> GetMyBookingsAsync();
    Task<ApiResponse<bool>> AcceptBookingAsync(Guid id);
    Task<ApiResponse<bool>> CompleteBookingAsync(Guid id);
    Task<ApiResponse<bool>> RejectBookingAsync(Guid id);
}
```

### MyBookings.razor
```csharp
- OnInitializedAsync() → LoadBookingsAsync()
- LoadBookingsAsync() → Appelle BookingService
- Affichage conditionnel basé sur IsLoading/Error/Empty
```

### BookingCard.razor
```csharp
- OnAcceptAsync() → AcceptBookingAsync() → Refresh
- OnCompleteAsync() → CompleteBookingAsync() → Refresh
- OnRejectAsync() → RejectBookingAsync() → Refresh
- ExecuteActionAsync() → Gestion centralisée des actions
```

---

## 🔐 Sécurité

1. **Authorization** : `@attribute [Authorize]` sur MyBookings.razor
2. **Token** : Automatiquement ajouté via AuthHeaderHandler
3. **Validation** : Côté backend pour toutes les actions

---

## 🚨 Gestion d'Erreurs

### Frontend
- Try/catch dans chaque méthode service
- Messages d'erreur utilisateur friendly
- Affichage visuel des erreurs (rouge, icône ⚠️)
- Bouton Retry disponible

### Backend
- Middleware ErrorHandlingMiddleware
- Exceptions métier (Not enough balance, etc.)
- Statut HTTP appropriés (400, 404, 500)

---

## 🎯 Prochaines Améliorations Possibles

1. **Filtres** : Par statut, par date
2. **Tri** : Plus récent, plus ancien
3. **Pagination** : Si > 20 bookings
4. **Détails** : Modal avec infos complètes
5. **Notifications** : Toast/Snackbar après actions
6. **Recherche** : Par ID de booking
7. **Export** : CSV/PDF de l'historique

---

## ✅ Checklist de Validation

- [x] BookingService créé et enregistré dans DI
- [x] Modèles BookingDto créés
- [x] Badge de statut avec couleurs
- [x] Badge escrow avec icônes
- [x] Carte de booking avec actions conditionnelles
- [x] Page MyBookings avec grille responsive
- [x] Gestion des états : Loading, Error, Empty, Success
- [x] Statistiques en temps réel
- [x] Design moderne et professionnel
- [x] Animations et transitions
- [x] Responsive design (mobile, tablet, desktop)
- [x] Gestion d'erreurs complète
- [x] Pas de breaking changes au backend

---

## 📞 Support

Si vous rencontrez des problèmes :
1. Vérifiez que l'API backend est en cours d'exécution
2. Vérifiez la console du navigateur pour les erreurs
3. Vérifiez que vous êtes authentifié
4. Vérifiez que le BaseAddress est correct (http://localhost:5001)

---

**🎉 Profitez de votre nouvelle interface My Bookings !**
