# Mobile Tools
![Mobile Tools Interface](https://github.com/elouafidev/MobileTools/blob/main/Capture6.JPG)
## Description
Mobile Tools est une application Windows permettant de diagnostiquer et de contrôler les téléphones Android. Elle utilise une bibliothèque personnalisée pour interagir avec les appareils via des commandes ADB (Android Debug Bridge).

## Fonctionnalités
- Affichage des informations du téléphone (numéro de série, IMEI, etc.)
- Diagnostique de la batterie, du stockage, et des performances
- Réinitialisation du téléphone
- Gestion des fichiers (copie, suppression, modification)
- Installation et gestion des applications
- Suppression des comptes (ex: compte Google)
- Terminal de commandes intégré pour exécuter des commandes directement sur le téléphone

## Installation
1. Clonez le dépôt GitHub :
   ```sh
   git clone https://github.com/elouafidev/MobileTools.git
   ```
2. Naviguez vers le répertoire du projet :
   ```sh
   cd MobileTools
   ```
3. Compilez et exécutez l'application selon les instructions fournies dans le projet.

## Utilisation
- Connectez votre téléphone Android à l'ordinateur via un câble USB.
- Activez le débogage USB sur votre téléphone.
- Lancez l'application et suivez les instructions à l'écran pour exécuter les différentes fonctionnalités.

## Exemples de Commandes

### Afficher les informations du téléphone
```sh
adb shell getprop
```

### Lister les applications installées
```sh
adb shell pm list packages
```

### Installer une application
```sh
adb install /path/to/your/app.apk
```

### Désinstaller une application
```sh
adb uninstall com.example.package
```

### Copier un fichier vers le téléphone
```sh
adb push /path/to/local/file /path/to/remote/location
```

### Supprimer un compte Google
```sh
adb shell pm clear com.google.android.gsf.login
```

### Réinitialiser le téléphone (factory reset)
```sh
adb shell am broadcast -a android.intent.action.MASTER_CLEAR
```

### Lancer une application spécifique
```sh
adb shell monkey -p com.example.package -c android.intent.category.LAUNCHER 1
```

## Contribution
Les contributions sont les bienvenues ! Si vous avez des idées pour améliorer l'application, n'hésitez pas à soumettre une pull request ou à me contacter directement.

## Contact
Pour toute question ou suggestion, vous pouvez me contacter par email : [support@elouafi.dev](mailto:support@elouafi.dev)

## Licence
Ce projet est sous licence MIT. Voir le fichier LICENSE pour plus de détails.
