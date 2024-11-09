CREATE DATABASE  IF NOT EXISTS `viajes` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `viajes`;
-- MySQL dump 10.13  Distrib 8.0.40, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: viajes
-- ------------------------------------------------------
-- Server version	9.1.0

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `buses`
--

DROP TABLE IF EXISTS `buses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `buses` (
  `idBus` int NOT NULL,
  `nombre` varchar(45) DEFAULT NULL,
  `placa` varchar(10) DEFAULT NULL,
  `asientos` int DEFAULT NULL,
  `idRuta` int DEFAULT NULL,
  `nombreRuta` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`idBus`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `buses`
--

LOCK TABLES `buses` WRITE;
/*!40000 ALTER TABLE `buses` DISABLE KEYS */;
INSERT INTO `buses` VALUES (1,'Doña Vilma','BUS073',52,1,'Naranjo - San José'),(2,'El Zarcereño','BUS125',52,2,'Zarcero - Naranjo'),(3,'La reina del norte','BUS365',52,3,'San Carlos - San José'),(4,'El principe azul','BUS730',42,4,'San Carlos - San José'),(5,'El moncheño','BUS328',42,8,'San Ramón - Naranjo'),(6,'El moncheño','BUS328',42,7,'Naranjo - San Ramón');
/*!40000 ALTER TABLE `buses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `pagos`
--

DROP TABLE IF EXISTS `pagos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `pagos` (
  `pk_tsal001` varchar(45) NOT NULL,
  `idReserva` int DEFAULT NULL,
  `terminalId` varchar(45) DEFAULT NULL,
  `transactionType` varchar(45) DEFAULT NULL,
  `invoice` varchar(45) DEFAULT NULL,
  `totalAmount` varchar(45) DEFAULT NULL,
  `taxAmount` varchar(45) DEFAULT NULL,
  `tipAmount` varchar(45) DEFAULT NULL,
  `clientEmail` varchar(45) DEFAULT NULL,
  `idCliente` int DEFAULT NULL,
  PRIMARY KEY (`pk_tsal001`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `pagos`
--

LOCK TABLES `pagos` WRITE;
/*!40000 ALTER TABLE `pagos` DISABLE KEYS */;
INSERT INTO `pagos` VALUES ('00600006010000000001',1,'EMVSBAT1','SALE','0000000001','1500','195','0','michael.acuna@uned.cr',206890200),('00600006010000000002',1,'EMVSBAT1','SALE','0000000002','1500','195','0','michael.acuna@uned.cr',206890200),('00600006010000000003',1,'EMVSBAT1','SALE','0000000003','1500','195','0','michael.acuna@uned.cr',206890200),('00600006010000000004',1,'EMVSBAT1','SALE','0000000004','1500','195','0','michael.acuna@uned.cr',206890200),('00600006010000000005',2,'EMVSBAT1','SALE','0000000005','1500','195','0','michael.acuna@uned.cr',206890200),('00600006010000000006',1,'EMVSBAT1','SALE','0000000006','1500','195','0','michael.acuna@uned.cr',206890200),('00600006010000000007',3,'EMVSBAT1','SALE','0000000007','1500','195','0','michael.acuna@uned.cr',206890200),('00600006010000000008',1,'EMVSBAT1','SALE','0000000008','1500','195','0','michael.acuna@uned.cr',206890200),('00600006010000000009',2,'EMVSBAT1','SALE','0000000009','1500','195','0','michael.acuna@uned.cr',206890200),('00600006010000000010',1,'EMVSBAT1','SALE','0000000010','1500','195','0','michael.acuna@uned.cr',206890200),('00600006010000000011',1,'EMVSBAT1','SALE','0000000011','1500','195','0','michael.acuna@uned.cr',206890200),('00600006010000000012',1,'EMVSBAT1','SALE','0000000012','1500','195','0','michael.acuna@uned.cr',206890200),('00600006010000000013',1,'EMVSBAT1','SALE','0000000013','1500','195','0','michael.acuna@uned.cr',206890200),('00600006010000000014',2,'EMVSBAT1','SALE','0000000014','1500','195','0','michael.acuna@uned.cr',206890200),('00600006010000000015',3,'EMVSBAT1','SALE','0000000015','1500','195','0','michael.acuna@uned.cr',206890200),('00600006010000000016',4,'EMVSBAT1','SALE','0000000016','1755','228,15','0','michael.acuna@uned.cr',206890200),('00600006010000000017',4,'EMVSBAT1','SALE','0000000017','1755','228,15','0','michael.acuna@uned.cr',206890200),('00600006010000000018',6,'EMVSBAT1','SALE','0000000018','8400','1092','0','michael.acuna@uned.cr',206890200),('00600006010000000019',7,'EMVSBAT1','SALE','0000000019','585','76,05','0','michael.acuna@uned.cr',206890200),('00600006010000000020',8,'EMVSBAT1','SALE','0000000020','3000','390','0','michael.acuna@uned.cr',206890200),('00600006010000000021',9,'EMVSBAT1','SALE','0000000021','710','92,3','0','michael.acuna@uned.cr',206890200),('00600006010000000022',3,'EMVSBAT1','SALE','0000000022','1500','195','0','michael.acuna@uned.cr',206890200);
/*!40000 ALTER TABLE `pagos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `reservas`
--

DROP TABLE IF EXISTS `reservas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `reservas` (
  `idReserva` int NOT NULL,
  `idUsuario` int DEFAULT NULL,
  `nombreUsuario` varchar(45) DEFAULT NULL,
  `idBus` int DEFAULT NULL,
  `nombreBus` varchar(45) DEFAULT NULL,
  `idRuta` int DEFAULT NULL,
  `nombreRuta` varchar(45) DEFAULT NULL,
  `asientosSeleccionados` varchar(100) DEFAULT NULL,
  `costo` int DEFAULT NULL,
  `estado` bit(1) DEFAULT NULL,
  PRIMARY KEY (`idReserva`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `reservas`
--

LOCK TABLES `reservas` WRITE;
/*!40000 ALTER TABLE `reservas` DISABLE KEYS */;
INSERT INTO `reservas` VALUES (1,206890200,'Maikol Acuña',1,'Doña Vilma',1,'Naranjo - San José','1',1500,_binary ''),(2,206890200,'Maikol Acuña',1,'Doña Vilma',1,'Naranjo - San José','2',1500,_binary '\0'),(3,206890200,'Maikol Acuña',1,'Doña Vilma',1,'Naranjo - San José','3',1500,_binary ''),(4,206890200,'Maikol Acuña',2,'El Zarcereño',2,'Zarcero - Naranjo','1,2,3',1755,_binary ''),(5,206890200,'Maikol Acuña',6,'El moncheño',7,'Naranjo - San Ramón','37,38,41,42',1420,_binary '\0'),(6,206890200,'Maikol Acuña',4,'El principe azul',4,'San José - San Carlos','37,38,42',8400,_binary ''),(7,206890200,'Maikol Acuña',2,'El Zarcereño',2,'Zarcero - Naranjo','4',585,_binary ''),(8,206890200,'Maikol Acuña',1,'Doña Vilma',1,'Naranjo - San José','49,50',3000,_binary ''),(9,206890200,'Maikol Acuña',5,'El moncheño',8,'San Ramón - Naranjo','1,2',710,_binary '');
/*!40000 ALTER TABLE `reservas` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `rutas`
--

DROP TABLE IF EXISTS `rutas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `rutas` (
  `idRuta` int NOT NULL,
  `nombreRuta` varchar(45) DEFAULT NULL,
  `origen` varchar(45) DEFAULT NULL,
  `destino` varchar(45) DEFAULT NULL,
  `fechaRuta` datetime DEFAULT NULL,
  `costo` decimal(8,2) DEFAULT NULL,
  PRIMARY KEY (`idRuta`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `rutas`
--

LOCK TABLES `rutas` WRITE;
/*!40000 ALTER TABLE `rutas` DISABLE KEYS */;
INSERT INTO `rutas` VALUES (1,'Naranjo - San José','Naranjo','San José','2024-11-04 08:15:00',1500.00),(2,'Zarcero - Naranjo','Zarcero','Naranjo','2024-11-04 07:00:00',585.00),(3,'San Carlos - San José','San Carlos','San José','2024-11-04 04:30:00',2800.00),(4,'San José - San Carlos','San José','San Carlos','2024-11-04 08:45:00',2800.00),(5,'San José - Naranjo','San José','Naranjo','2024-11-11 10:00:00',1500.00),(6,'Naranjo - Zarcero','Naranjo','Zarcero','2024-11-11 13:15:00',585.00),(7,'Naranjo - San Ramón','Naranjo','San Ramón','2024-11-11 14:00:00',355.00),(8,'San Ramón - Naranjo','San Ramón','Naranjo','2024-11-11 14:40:00',355.00);
/*!40000 ALTER TABLE `rutas` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `usuarios`
--

DROP TABLE IF EXISTS `usuarios`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `usuarios` (
  `idUsuario` int NOT NULL,
  `nombre` varchar(45) DEFAULT NULL,
  `correo` varchar(45) DEFAULT NULL,
  `password` varchar(65) DEFAULT NULL,
  PRIMARY KEY (`idUsuario`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `usuarios`
--

LOCK TABLES `usuarios` WRITE;
/*!40000 ALTER TABLE `usuarios` DISABLE KEYS */;
INSERT INTO `usuarios` VALUES (206890200,'Maikol Acuña','michael.acuna@uned.cr','4a44dc15364204a80fe80e9039455cc1608281820fe2b24f1e5233ade6af1dd5');
/*!40000 ALTER TABLE `usuarios` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2024-11-09 17:14:20
